namespace TodoListApp.WebApp.Services;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListData>> GetAllAsync();

    Task<TodoListData?> GetByIdAsync(int id);

    Task CreateAsync(TodoListData todoList);

    Task UpdateAsync(int id, TodoListData todoList);

    Task DeleteAsync(int id);
}
