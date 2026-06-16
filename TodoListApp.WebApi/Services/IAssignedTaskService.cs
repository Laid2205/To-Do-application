using TodoListApp.DataAccess.Entities;

namespace TodoListApp.WebApi.Services;

public interface IAssignedTaskService
{
    Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy);

    Task UpdateStatusAsync(int taskId, string assigneeId, TodoTaskStatus newStatus);
}
