using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class CommentWebApiService : ICommentWebApiService
{
    private readonly HttpClient httpClient;

    public CommentWebApiService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<IEnumerable<CommentData>> GetByTaskIdAsync(int taskId)
    {
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<CommentWebApiModel>>(
            $"api/tasks/{taskId}/comments");
        return models?.Select(m => new CommentData
        {
            Id = m.Id,
            Text = m.Text,
            CreatedAt = m.CreatedAt,
            TaskId = m.TaskId,
        }) ?? Enumerable.Empty<CommentData>();
    }

    public async Task CreateAsync(int taskId, string text)
    {
        await this.httpClient.PostAsJsonAsync($"api/tasks/{taskId}/comments", text);
    }

    public async Task UpdateAsync(int taskId, int commentId, string text)
    {
        await this.httpClient.PutAsJsonAsync($"api/tasks/{taskId}/comments/{commentId}", text);
    }

    public async Task DeleteAsync(int taskId, int commentId)
    {
        await this.httpClient.DeleteAsync(new Uri(
            this.httpClient.BaseAddress!, $"api/tasks/{taskId}/comments/{commentId}"));
    }
}
