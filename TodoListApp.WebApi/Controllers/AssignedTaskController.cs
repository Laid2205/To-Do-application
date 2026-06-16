using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.DataAccess.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/assignedtasks")]
public class AssignedTaskController : ControllerBase
{
    private static readonly Action<ILogger, int, string, Exception?> LogTaskNotFoundForAssignee =
        LoggerMessage.Define<int, string>(LogLevel.Warning, new EventId(1, "TaskNotFoundForAssignee"), "Task {TaskId} not found for assignee {AssigneeId}.");

    private readonly IAssignedTaskService service;
    private readonly ILogger<AssignedTaskController> logger;

    public AssignedTaskController(IAssignedTaskService service, ILogger<AssignedTaskController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet("{assigneeId}")]
    public async Task<ActionResult<IEnumerable<TodoTaskModel>>> GetAssignedTasks(
        string assigneeId,
        [FromQuery] string? statusFilter = null,
        [FromQuery] string? sortBy = null)
    {
        var tasks = await this.service.GetAssignedTasksAsync(assigneeId, statusFilter, sortBy);

        return this.Ok(tasks.Select(t => new TodoTaskModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            TodoListId = t.TodoListId,
            CategoryId = t.CategoryId,
            CategoryName = t.CategoryName,
        }));
    }

    [HttpPatch("{assigneeId}/tasks/{taskId}/status")]
    public async Task<IActionResult> UpdateStatus(string assigneeId, int taskId, [FromBody] TodoTaskStatus newStatus)
    {
        try
        {
            await this.service.UpdateStatusAsync(taskId, assigneeId, newStatus);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTaskNotFoundForAssignee(this.logger, taskId, assigneeId, ex);
            return this.NotFound();
        }
    }
}
