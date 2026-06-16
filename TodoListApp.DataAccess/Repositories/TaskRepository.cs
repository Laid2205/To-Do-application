using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Data;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoListDbContext context;

    public TaskRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllByListIdAsync(int todoListId)
    {
        return await this.context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.TodoListId == todoListId)
            .ToListAsync();
    }

    public async Task<PagedEntityResult<TaskEntity>> GetFilteredAsync(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber,
        int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = this.context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.TodoListId == todoListId);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(t =>
                t.Title.Contains(searchText) ||
                t.Description.Contains(searchText));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(t =>
                t.Category != null && t.Category.Name == category);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedEntityResult<TaskEntity>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<TaskEntity?> GetByIdAsync(int id)
    {
        return await this.context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskEntity> CreateAsync(TaskEntity entity)
    {
        this.context.Tasks.Add(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TaskEntity entity)
    {
        this.context.Tasks.Update(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.Tasks.FindAsync(id)
            ?? throw new KeyNotFoundException($"Task with id {id} not found.");

        this.context.Tasks.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskEntity>> SearchByTitleAsync(string searchText)
    {
        return await this.context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.Title.Contains(searchText))
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetAssignedTasksAsync(string assigneeId, string? statusFilter, string? sortBy)
    {
        var query = this.context.Tasks
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.AssigneeId == assigneeId);

        if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<TodoTaskStatus>(statusFilter, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        query = sortBy switch
        {
            "duedate" => query.OrderBy(t => t.DueDate),
            "status" => query.OrderBy(t => t.Status),
            "title" => query.OrderBy(t => t.Title),
            _ => query.OrderBy(t => t.CreatedAt),
        };

        return await query.ToListAsync();
    }

    public async Task UpdateStatusAsync(int taskId, string assigneeId, TodoTaskStatus newStatus)
    {
        var entity = await this.context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.AssigneeId == assigneeId)
            ?? throw new KeyNotFoundException($"Task with id {taskId} not found for assignee {assigneeId}.");

        entity.Status = newStatus;
        await this.context.SaveChangesAsync();
    }
}
