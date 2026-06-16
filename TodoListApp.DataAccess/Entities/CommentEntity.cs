namespace TodoListApp.DataAccess.Entities;

public class CommentEntity
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;
}
