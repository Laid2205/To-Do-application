using System.Collections.ObjectModel;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Models;

public class TodoTaskWebApiModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public TodoTaskStatus Status { get; set; }

    public string AssigneeId { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public Collection<TagWebApiModel> Tags { get; } = new Collection<TagWebApiModel>();

    public Collection<CommentWebApiModel> Comments { get; } = new Collection<CommentWebApiModel>();

    public bool IsOverdue => this.DueDate.HasValue
        && this.DueDate.Value < DateTime.UtcNow
        && this.Status != TodoTaskStatus.Completed;
}
