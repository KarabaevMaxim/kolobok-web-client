using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Dto;

namespace KolobokClient
{
  public class WebClient : IDisposable
  {
    private readonly HttpClient _client;
    
    public async Task<TResponse?> Post<TData, TResponse>(TData data, Uri uri, string? accessToken, CancellationToken cancellationToken) where TResponse : ServerResponse
    {
      SetAuthorizationHeaderIfNeeded(accessToken);
      var serialized = JsonConvert.SerializeObject(data);
      var result = await _client.PostAsync(uri, new StringContent(serialized, Encoding.Default, MediaTypeNames.Application.Json), cancellationToken);
      return await HandleResponse<TResponse>(result, uri);
    }

    public async Task<TResponse?> Get<TResponse>(Uri uri, string? accessToken, CancellationToken cancellationToken) where TResponse : ServerResponse
    {
      SetAuthorizationHeaderIfNeeded(accessToken);
      var result = await _client.GetAsync(uri, cancellationToken);
      return await HandleResponse<TResponse>(result, uri);
    }
    
    public Uri ConstructUri(bool useHttps, string host, int port, string apiPath, string controller, string method, Dictionary<string, string>? query = null)
    {
      var uriBuilder = new UriBuilder();
      uriBuilder.Scheme = useHttps ? "https" : "http";
      uriBuilder.Host = host;
      uriBuilder.Port = port;
      uriBuilder.Path = $"{apiPath}/{controller}/{method}";
      
      if (query != null)
      {
        var sb = new StringBuilder();
        var i = 0;
        
        foreach (var param in query)
        {
          sb.Append($"{param.Key}={param.Value}");
          
          if (i < query.Count)
            sb.Append("&");
          
          i++;
        }

        uriBuilder.Query = sb.ToString();
      }
      
      return uriBuilder.Uri;
    }

    public void Dispose()
    {
      _client.Dispose();
    }

    private void SetAuthorizationHeaderIfNeeded(string? token)
    {
      if (string.IsNullOrEmpty(token))
        return;
      
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<TResponse?> HandleResponse<TResponse>(HttpResponseMessage? response, Uri uri) where TResponse : ServerResponse
    {
      if (response == null)
      {
        Console.WriteLine($"ERROR | Ответ не получен");
        return default;
      }
        
      if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
        return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
      
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        var content = await response.Content.ReadAsStringAsync();

        if (content == null)
        {
          Console.WriteLine($"ERROR | Указанный {uri} не найден");
          return default;
        }
        
        return JsonConvert.DeserializeObject<TResponse>(content);
      }
      
      Console.WriteLine($"ERROR | Ошибка {response.StatusCode}. {response.ReasonPhrase}");
      return default;
    }

    public WebClient()
    {
      _client = new HttpClient();
    }
  }
}