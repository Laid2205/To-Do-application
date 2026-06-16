namespace TodoListApp.WebApp.Services;

public interface IAssignedTaskWebApiService
{
    Task<IEnumerable<TodoTaskData>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy);

    Task UpdateStatusAsync(string assigneeId, int taskId, TodoTaskStatus newStatus);
}
