namespace TodoListApp.DataAccess.Entities;

public class TaskEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public TodoTaskStatus Status { get; set; } = TodoTaskStatus.NotStarted;

    public string AssigneeId { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;

    public int? CategoryId { get; set; }

    public CategoryEntity? Category { get; set; }
}

public enum TodoTaskStatus
{
    NotStarted,
    InProgress,
    Completed,
}
