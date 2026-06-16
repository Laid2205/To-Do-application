namespace TodoListApp.WebApi.Services;

public class Tag
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public int TaskId { get; set; }
}
