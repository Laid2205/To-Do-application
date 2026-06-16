namespace TodoListApp.WebApi.Services;

public interface ITaskDatabaseService
{
    Task<IEnumerable<TodoTask>> GetAllByListIdAsync(int todoListId);

    Task<PagedResult<TodoTask>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize);

    Task<TodoTask?> GetByIdAsync(int id);

    Task<TodoTask> CreateAsync(TodoTask task);

    Task UpdateAsync(int id, TodoTask task);

    Task DeleteAsync(int id);

    Task<IEnumerable<TodoTask>> SearchByTitleAsync(string searchText);
}
