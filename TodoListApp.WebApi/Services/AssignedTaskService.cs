using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class AssignedTaskService : IAssignedTaskService
{
    private readonly ITaskRepository taskRepository;

    public AssignedTaskService(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TodoTask>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy)
    {
        var entities = await this.taskRepository.GetAssignedTasksAsync(assigneeId, statusFilter, sortBy);

        return entities.Select(t => new TodoTask
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            TodoListId = t.TodoListId,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name,
        });
    }

    public Task UpdateStatusAsync(int taskId, string assigneeId, TodoTaskStatus newStatus)
    {
        return this.taskRepository.UpdateStatusAsync(taskId, assigneeId, newStatus);
    }
}
