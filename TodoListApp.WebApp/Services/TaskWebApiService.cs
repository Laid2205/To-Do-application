using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient httpClient;

    public TaskWebApiService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<IEnumerable<TodoTaskData>> GetAllByListIdAsync(int todoListId)
    {
        var result = await this.GetFilteredAsync(todoListId, null, null, 1, 100);
        return result.Items;
    }

    public async Task<PagedResultData<TodoTaskData>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize)
    {
        var query = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}",
        };

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query.Add($"searchText={Uri.EscapeDataString(searchText)}");
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query.Add($"category={Uri.EscapeDataString(category)}");
        }

        var response = await this.httpClient.GetAsync(new Uri(
            this.httpClient.BaseAddress!, $"api/todolists/{todoListId}/tasks?{string.Join("&", query)}"));
        if (!response.IsSuccessStatusCode)
        {
            return new PagedResultData<TodoTaskData>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        var model = await response.Content.ReadFromJsonAsync<PagedResultWebApiModel<TodoTaskWebApiModel>>();
        return new PagedResultData<TodoTaskData>
        {
            Items = model?.Items.Select(m => MapToData(m)) ?? Enumerable.Empty<TodoTaskData>(),
            PageNumber = model?.PageNumber ?? pageNumber,
            PageSize = model?.PageSize ?? pageSize,
            TotalCount = model?.TotalCount ?? 0,
        };
    }

    public async Task<TodoTaskData?> GetByIdAsync(int todoListId, int id)
    {
        var response = await this.httpClient.GetAsync(new Uri(
            this.httpClient.BaseAddress!, $"api/todolists/{todoListId}/tasks/{id}"));
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var model = await response.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();
        return model == null ? null : MapToData(model);
    }

    public Task CreateAsync(TodoTaskData task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return this.CreateAsyncCore(task);
    }

    public Task UpdateAsync(int todoListId, int id, TodoTaskData task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return this.UpdateAsyncCore(todoListId, id, task);
    }

    public async Task DeleteAsync(int todoListId, int id)
    {
        var response = await this.httpClient.DeleteAsync(new Uri(
            this.httpClient.BaseAddress!, $"api/todolists/{todoListId}/tasks/{id}"));
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TodoTaskData>> SearchByTitleAsync(string searchText)
    {
        var response = await this.httpClient.GetAsync(new Uri(
            this.httpClient.BaseAddress!,
            $"api/tasks/search?searchText={Uri.EscapeDataString(searchText)}"));
        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<TodoTaskData>();
        }

        var models = await response.Content.ReadFromJsonAsync<IEnumerable<TodoTaskWebApiModel>>();
        return models?.Select(m => MapToData(m)) ?? Enumerable.Empty<TodoTaskData>();
    }

    private static TodoTaskData MapToData(TodoTaskWebApiModel model)
    {
        return new TodoTaskData
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            CreatedAt = model.CreatedAt,
            DueDate = model.DueDate,
            Status = model.Status,
            AssigneeId = model.AssigneeId,
            TodoListId = model.TodoListId,
        };
    }

    private static TodoTaskWebApiModel MapToModel(TodoTaskData data)
    {
        return new TodoTaskWebApiModel
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            DueDate = data.DueDate,
            Status = data.Status,
            AssigneeId = data.AssigneeId,
            TodoListId = data.TodoListId,
        };
    }

    private async Task CreateAsyncCore(TodoTaskData task)
    {
        var model = MapToModel(task);
        var response = await this.httpClient.PostAsJsonAsync(
            $"api/todolists/{task.TodoListId}/tasks", model);
        response.EnsureSuccessStatusCode();
    }

    private async Task UpdateAsyncCore(int todoListId, int id, TodoTaskData task)
    {
        var model = MapToModel(task);
        var response = await this.httpClient.PutAsJsonAsync(
            $"api/todolists/{todoListId}/tasks/{id}", model);
        response.EnsureSuccessStatusCode();
    }
}
