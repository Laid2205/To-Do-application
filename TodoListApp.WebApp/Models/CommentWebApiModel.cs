namespace TodoListApp.WebApp.Models;

public class CommentWebApiModel
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int TaskId { get; set; }

    public int TodoListId { get; set; }
}
