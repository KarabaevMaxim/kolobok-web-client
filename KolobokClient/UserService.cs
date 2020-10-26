using System;
using System.Threading.Tasks;
using Shared.Dto;

namespace KolobokClient
{
  public class UserService
  {
    private readonly WebClient _client;

    private string? _token;
    private Guid? _userId;
    
    public async Task<(bool, string?)> Register(string device, string os, Guid deviceId)
    {
      var uri = _client.ConstructUri(Params.UseHttps, Params.Host, Params.Port, Params.ApiPath, "user", "register");

      var dto = new RegisterUserViewModel
      {
        Device = device,
        OS = os,
        UniqueDeviceId = deviceId
      };
      var result = await _client.Post<RegisterUserViewModel, LoginServerResponse>(dto, uri);

      if (result == null) // значит ошибка на уровне протокола
        return (false, "Не удалось установить связь с сервером");

      if (result.IsSuccess)
      {
        _token = result.AccessToken;
        _userId = Guid.Parse(result.UserId!);
        Console.Write($"Регистрация пройдена. Токен доступа: {_token}, идентификатор пользователя: {_userId}");
        return (true, string.Empty);
      }

      return (false, result.Message);
    }
    
    public UserService()
    {
      _client = new WebClient();
    }
  }
}