namespace TodoListApp.WebApp.Services;

public interface IWebApiAuthService
{
    Task<string?> LoginAsync(string email, string password);

    Task<string?> RegisterAsync(string email, string password);
}
