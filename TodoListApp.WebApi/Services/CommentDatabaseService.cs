using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class CommentDatabaseService : ICommentDatabaseService
{
    private readonly ICommentRepository commentRepository;

    public CommentDatabaseService(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId)
    {
        var entities = await this.commentRepository.GetByTaskIdAsync(taskId);
        return entities.Select(c => new Comment
        {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            TaskId = c.TaskId,
        });
    }

    public Task<Comment> CreateAsync(int taskId, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return this.CreateAsyncCore(taskId, text);
    }

    public Task UpdateAsync(int commentId, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return this.UpdateAsyncCore(commentId, text);
    }

    public Task DeleteAsync(int commentId)
    {
        return this.commentRepository.DeleteAsync(commentId);
    }

    private async Task<Comment> CreateAsyncCore(int taskId, string text)
    {
        var entity = new CommentEntity
        {
            Text = text,
            TaskId = taskId,
            CreatedAt = DateTime.UtcNow,
        };

        var created = await this.commentRepository.CreateAsync(entity);
        return new Comment
        {
            Id = created.Id,
            Text = created.Text,
            CreatedAt = created.CreatedAt,
            TaskId = created.TaskId,
        };
    }

    private async Task UpdateAsyncCore(int commentId, string text)
    {
        var entity = await this.commentRepository.GetByIdAsync(commentId)
            ?? throw new KeyNotFoundException($"Comment with id {commentId} not found.");

        entity.Text = text;
        await this.commentRepository.UpdateAsync(entity);
    }
}
