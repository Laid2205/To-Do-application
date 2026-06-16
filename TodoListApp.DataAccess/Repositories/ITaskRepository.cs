using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllByListIdAsync(int todoListId);

    Task<PagedEntityResult<TaskEntity>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize);

    Task<TaskEntity?> GetByIdAsync(int id);

    Task<TaskEntity> CreateAsync(TaskEntity entity);

    Task UpdateAsync(TaskEntity entity);

    Task DeleteAsync(int id);

    Task<IEnumerable<TaskEntity>> SearchByTitleAsync(string searchText);

    Task<IEnumerable<TaskEntity>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy);

    Task UpdateStatusAsync(int taskId, string assigneeId, TodoTaskStatus newStatus);
}
