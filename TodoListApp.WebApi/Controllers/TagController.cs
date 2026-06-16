using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private static readonly Action<ILogger, int, int, Exception?> LogTagNotFound =
        LoggerMessage.Define<int, int>(LogLevel.Warning, new EventId(1, "TagNotFound"), "Tag {TagId} not found for task {TaskId}.");

    private readonly ITagDatabaseService service;
    private readonly ILogger<TagController> logger;

    public TagController(ITagDatabaseService service, ILogger<TagController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetAll()
    {
        var tags = await this.service.GetAllAsync();
        return this.Ok(tags.Select(t => new TagModel { Id = t.Id, Label = t.Label, TaskId = t.TaskId }));
    }

    [HttpGet("tasks/{taskId}")]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetByTaskId(int taskId)
    {
        var tags = await this.service.GetByTaskIdAsync(taskId);
        return this.Ok(tags.Select(t => new TagModel { Id = t.Id, Label = t.Label, TaskId = t.TaskId }));
    }

    [HttpGet("bytag")]
    public async Task<ActionResult<IEnumerable<TodoTaskModel>>> GetTasksByTag([FromQuery] string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return this.BadRequest("Label cannot be empty.");
        }

        var tasks = await this.service.GetTasksByTagAsync(label);
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
        }));
    }

    [HttpPost("tasks/{taskId}")]
    public async Task<ActionResult<TagModel>> AddTag(int taskId, [FromBody] string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return this.BadRequest("Label cannot be empty.");
        }

        var tag = await this.service.AddTagToTaskAsync(taskId, label);
        return this.CreatedAtAction(nameof(this.GetByTaskId), new { taskId }, new TagModel
        {
            Id = tag.Id,
            Label = tag.Label,
            TaskId = tag.TaskId,
        });
    }

    [HttpDelete("tasks/{taskId}/{tagId}")]
    public async Task<IActionResult> RemoveTag(int taskId, int tagId)
    {
        try
        {
            await this.service.RemoveTagFromTaskAsync(taskId, tagId);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTagNotFound(this.logger, tagId, taskId, ex);
            return this.NotFound();
        }
    }
}
