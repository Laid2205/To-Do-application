namespace TodoListApp.WebApi.Models;

public class CommentModel
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int TaskId { get; set; }
}
