using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TodoListApp.WebApp.Services;

public class WebApiAuthService : IWebApiAuthService
{
    private readonly HttpClient httpClient;

    public WebApiAuthService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/auth/login", new
        {
            Email = email,
            Password = password,
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth?.Token;
    }

    public async Task<string?> RegisterAsync(string email, string password)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/auth/register", new
        {
            Email = email,
            Password = password,
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth?.Token;
    }

    private sealed class AuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
