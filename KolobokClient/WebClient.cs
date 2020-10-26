using System;
using System.Collections.Generic;
using System.Text;

namespace KolobokClient
{
  public class WebClient
  {
    public Uri ConstructUri(bool useHttps, string host, int port, string apiPath, string controller, string method, Dictionary<string, string> query = null)
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
  }
}