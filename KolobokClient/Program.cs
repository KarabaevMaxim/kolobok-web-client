using System;
using System.Threading.Tasks;

namespace KolobokClient
{
  internal static class Program
  {
    static async Task Main(string[] args)
    {
      var userService = new UserService();
      var (success, message) = await userService.Register("My mac", "Mac 10.15", Guid.NewGuid());
      
      if (!success)
        Console.WriteLine($"Не удалось зарегистрироваться. Причина: {message}");
    }
  }
}