using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class AssignedTaskWebApiService : IAssignedTaskWebApiService
{
    private readonly HttpClient httpClient;

    public AssignedTaskWebApiService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<IEnumerable<TodoTaskData>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy)
    {
        var url = $"api/assignedtasks/{assigneeId}?statusFilter={statusFilter}&sortBy={sortBy}";
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<AssignedTaskWebApiModel>>(url);
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

    public async Task UpdateStatusAsync(string assigneeId, int taskId, TodoTaskStatus newStatus)
    {
        await this.httpClient.PatchAsJsonAsync(
            $"api/assignedtasks/{assigneeId}/tasks/{taskId}/status",
            newStatus);
    }
}
