namespace TodoListApp.WebApp.Services;

public interface ITaskWebApiService
{
    Task<IEnumerable<TodoTaskData>> GetAllByListIdAsync(int todoListId);

    Task<PagedResultData<TodoTaskData>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize);

    Task<TodoTaskData?> GetByIdAsync(int todoListId, int id);

    Task CreateAsync(TodoTaskData task);

    Task UpdateAsync(int todoListId, int id, TodoTaskData task);

    Task DeleteAsync(int todoListId, int id);

    Task<IEnumerable<TodoTaskData>> SearchByTitleAsync(string searchText);
}
