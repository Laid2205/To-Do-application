using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class TaskDatabaseService : ITaskDatabaseService
{
    private readonly ITaskRepository taskRepository;

    public TaskDatabaseService(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TodoTask>> GetAllByListIdAsync(int todoListId)
    {
        var entities = await this.taskRepository.GetAllByListIdAsync(todoListId);
        return entities.Select(MapToService);
    }

    public async Task<PagedResult<TodoTask>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize)
    {
        var result = await this.taskRepository.GetFilteredAsync(
            todoListId, searchText, category, pageNumber, pageSize);

        return new PagedResult<TodoTask>
        {
            Items = result.Items.Select(MapToService),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
        };
    }

    public async Task<TodoTask?> GetByIdAsync(int id)
    {
        var entity = await this.taskRepository.GetByIdAsync(id);
        return entity == null ? null : MapToService(entity);
    }

    public Task<TodoTask> CreateAsync(TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return this.CreateAsyncCore(task);
    }

    public Task UpdateAsync(int id, TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return this.UpdateAsyncCore(id, task);
    }

    public Task DeleteAsync(int id)
    {
        return this.taskRepository.DeleteAsync(id);
    }

    public Task<IEnumerable<TodoTask>> SearchByTitleAsync(string searchText)
    {
        ArgumentNullException.ThrowIfNull(searchText);
        return this.SearchByTitleAsyncCore(searchText);
    }

    private static TodoTask MapToService(TaskEntity entity)
    {
        return new TodoTask
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            AssigneeId = entity.AssigneeId,
            TodoListId = entity.TodoListId,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category?.Name,
        };
    }

    private async Task<TodoTask> CreateAsyncCore(TodoTask task)
    {
        var entity = new TaskEntity
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            TodoListId = task.TodoListId,
            CategoryId = task.CategoryId,
            CreatedAt = DateTime.UtcNow,
        };

        var created = await this.taskRepository.CreateAsync(entity);
        task.Id = created.Id;
        task.CreatedAt = created.CreatedAt;
        return task;
    }

    private async Task UpdateAsyncCore(int id, TodoTask task)
    {
        var entity = await this.taskRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Task with id {id} not found.");

        entity.Title = task.Title;
        entity.Description = task.Description;
        entity.DueDate = task.DueDate;
        entity.Status = task.Status;
        entity.AssigneeId = task.AssigneeId;
        entity.CategoryId = task.CategoryId;

        await this.taskRepository.UpdateAsync(entity);
    }

    private async Task<IEnumerable<TodoTask>> SearchByTitleAsyncCore(string searchText)
    {
        var entities = await this.taskRepository.SearchByTitleAsync(searchText);
        return entities.Select(MapToService);
    }
}
