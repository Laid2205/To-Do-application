namespace TodoListApp.WebApi.Services;

public interface ITodoListDatabaseService
{
    Task<IEnumerable<TodoList>> GetAllAsync();

    Task<TodoList?> GetByIdAsync(int id);

    Task<TodoList> CreateAsync(TodoList todoList);

    Task UpdateAsync(int id, TodoList todoList);

    Task DeleteAsync(int id);
}
