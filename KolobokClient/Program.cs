using System;
using System.Threading.Tasks;

namespace KolobokClient
{
  internal static class Program
  {
    static async Task Main(string[] args)
    {
      var userService = new UserService();
      var deviceId = Guid.NewGuid();
      var (success1, message1) = await userService.Register("My mac", "Mac 10.15", deviceId);
      
      if (!success1)
        Console.WriteLine($"Не удалось зарегистрироваться. Причина: {message1}");
      
      var (success2, message2) = await userService.Authorize(deviceId);
      
      if (!success2)
        Console.WriteLine($"Не удалось авторизоваться. Причина: {message2}");
      
      var (success3, message3) = await userService.GetCurrentUser();
      
      if (!success3)
        Console.WriteLine($"Не удалось получить пользователя. Причина: {message3}");
    }
  }
}