namespace TodoListApp.WebApp.Services;

public class CommentData
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int TaskId { get; set; }
}

public interface ICommentWebApiService
{
    Task<IEnumerable<CommentData>> GetByTaskIdAsync(int taskId);

    Task CreateAsync(int taskId, string text);

    Task UpdateAsync(int taskId, int commentId, string text);

    Task DeleteAsync(int taskId, int commentId);
}
