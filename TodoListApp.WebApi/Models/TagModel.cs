namespace TodoListApp.WebApi.Models;

public class TagModel
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public int TaskId { get; set; }
}
