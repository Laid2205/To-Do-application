using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;

    public TodoListWebApiService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        this.httpClient = httpClientFactory.CreateClient("WebApi");
    }

    public async Task<IEnumerable<TodoListData>> GetAllAsync()
    {
        var models = await this.httpClient.GetFromJsonAsync<IEnumerable<TodoListWebApiModel>>("api/todolist");
        return models?.Select(m => new TodoListData { Id = m.Id, Title = m.Title, Description = m.Description })
            ?? Enumerable.Empty<TodoListData>();
    }

    public async Task<TodoListData?> GetByIdAsync(int id)
    {
        var model = await this.httpClient.GetFromJsonAsync<TodoListWebApiModel>($"api/todolist/{id}");
        return model == null ? null : new TodoListData { Id = model.Id, Title = model.Title, Description = model.Description };
    }

    public Task CreateAsync(TodoListData todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        return this.CreateAsyncCore(todoList);
    }

    public Task UpdateAsync(int id, TodoListData todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        return this.UpdateAsyncCore(id, todoList);
    }

    public async Task DeleteAsync(int id)
    {
        await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress!, $"api/todolist/{id}"));
    }

    private async Task CreateAsyncCore(TodoListData todoList)
    {
        var model = new TodoListWebApiModel { Title = todoList.Title, Description = todoList.Description };
        await this.httpClient.PostAsJsonAsync("api/todolist", model);
    }

    private async Task UpdateAsyncCore(int id, TodoListData todoList)
    {
        var model = new TodoListWebApiModel { Title = todoList.Title, Description = todoList.Description };
        await this.httpClient.PutAsJsonAsync($"api/todolist/{id}", model);
    }
}
