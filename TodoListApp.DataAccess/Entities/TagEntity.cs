namespace TodoListApp.DataAccess.Entities;

public class TagEntity
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;
}
