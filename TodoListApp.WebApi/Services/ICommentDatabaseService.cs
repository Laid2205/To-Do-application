namespace TodoListApp.WebApi.Services;

public interface ICommentDatabaseService
{
    Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId);

    Task<Comment> CreateAsync(int taskId, string text);

    Task UpdateAsync(int commentId, string text);

    Task DeleteAsync(int commentId);
}
