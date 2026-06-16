namespace TodoListApp.WebApp.Models;

public class AssignedTaskWebApiModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public Services.TodoTaskStatus Status { get; set; }

    public string AssigneeId { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public bool IsOverdue => this.DueDate.HasValue
        && this.DueDate.Value < DateTime.UtcNow
        && this.Status != Services.TodoTaskStatus.Completed;
}
