namespace TodoListApp.WebApp.Models;

public class TagWebApiModel
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public int TaskId { get; set; }
}
