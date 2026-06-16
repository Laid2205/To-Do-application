using TodoListApp.DataAccess.Entities;

namespace TodoListApp.WebApi.Services;

public class TodoTask
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public TodoTaskStatus Status { get; set; }

    public string AssigneeId { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }
}
