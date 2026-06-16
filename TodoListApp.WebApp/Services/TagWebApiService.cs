using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TagWebApiService : ITagWebApiService
{
    private readonly HttpClient httpClient;

    public TagWebApiService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<IEnumerable<TagData>> GetAllAsync()
    {
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TagWebApiModel>>("api/tags");
        return models?.Select(m => new TagData { Id = m.Id, Label = m.Label, TaskId = m.TaskId })
            ?? Enumerable.Empty<TagData>();
    }

    public async Task<IEnumerable<TagData>> GetByTaskIdAsync(int taskId)
    {
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TagWebApiModel>>(
            $"api/tags/tasks/{taskId}");
        return models?.Select(m => new TagData { Id = m.Id, Label = m.Label, TaskId = m.TaskId })
            ?? Enumerable.Empty<TagData>();
    }

    public async Task<IEnumerable<TodoTaskData>> GetTasksByTagAsync(string label)
    {
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>($"api/tags/bytag?label={Uri.EscapeDataString(label)}");
        return models?.Select(m => new TodoTaskData
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            CreatedAt = m.CreatedAt,
            DueDate = m.DueDate,
            Status = m.Status,
            AssigneeId = m.AssigneeId,
            TodoListId = m.TodoListId,
        }) ?? Enumerable.Empty<TodoTaskData>();
    }

    public async Task AddTagToTaskAsync(int taskId, string label)
    {
        await this.httpClient.PostAsJsonAsync($"api/tags/tasks/{taskId}", label);
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress!, $"api/tags/tasks/{taskId}/{tagId}"));
    }
}
