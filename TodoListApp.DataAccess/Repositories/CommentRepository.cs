using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Data;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly TodoListDbContext context;

    public CommentRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<CommentEntity>> GetByTaskIdAsync(int taskId)
    {
        return await this.context.Comments
            .AsNoTracking()
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<CommentEntity?> GetByIdAsync(int id)
    {
        return await this.context.Comments.FindAsync(id);
    }

    public async Task<CommentEntity> CreateAsync(CommentEntity entity)
    {
        this.context.Comments.Add(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(CommentEntity entity)
    {
        this.context.Comments.Update(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.Comments.FindAsync(id)
            ?? throw new KeyNotFoundException($"Comment with id {id} not found.");

        this.context.Comments.Remove(entity);
        await this.context.SaveChangesAsync();
    }
}
