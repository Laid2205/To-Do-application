using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Data;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TodoListDbContext context;

    public TagRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagEntity>> GetAllAsync()
    {
        return await this.context.Tags.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TagEntity>> GetByTaskIdAsync(int taskId)
    {
        return await this.context.Tags
            .AsNoTracking()
            .Where(t => t.TaskId == taskId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByTagAsync(string label)
    {
        return await this.context.Tags
            .AsNoTracking()
            .Where(t => t.Label == label)
            .Select(t => t.Task)
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<TagEntity> AddTagToTaskAsync(int taskId, string label)
    {
        var entity = new TagEntity { Label = label, TaskId = taskId };
        this.context.Tags.Add(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        var entity = await this.context.Tags
            .FirstOrDefaultAsync(t => t.Id == tagId && t.TaskId == taskId)
            ?? throw new KeyNotFoundException($"Tag with id {tagId} not found for task {taskId}.");

        this.context.Tags.Remove(entity);
        await this.context.SaveChangesAsync();
    }
}
