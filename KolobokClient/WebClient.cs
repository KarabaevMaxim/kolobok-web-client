using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Dto;

namespace KolobokClient
{
  public class WebClient : IDisposable
  {
    private readonly HttpClient _client;
    
    public async Task<TResponse?> Post<TData, TResponse>(TData data, Uri uri, string? token = null) 
      where TData : IDto
      where TResponse : ServerResponse
    {
      var serialized = JsonConvert.SerializeObject(data);
      
      if (token != null)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      
      var result = await _client.PostAsync(uri, new StringContent(serialized, Encoding.Default, MediaTypeNames.Application.Json));

      if (result.IsSuccessStatusCode || result.StatusCode == HttpStatusCode.BadRequest)
        return JsonConvert.DeserializeObject<TResponse>(await result.Content.ReadAsStringAsync());

      if (result.StatusCode == HttpStatusCode.NotFound)
      {
        var content = await result.Content.ReadAsStringAsync();

        if (content == null)
        {
          Console.WriteLine($"ERROR | Указанный {uri} не найден");
          return default;
        }
        
        return JsonConvert.DeserializeObject<TResponse>(content);
      }
      
      Console.WriteLine($"ERROR | Ошибка {result.StatusCode}. {result.ReasonPhrase}");
      return default;
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

    public WebClient()
    {
      _client = new HttpClient();
    }
  }
}