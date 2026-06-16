using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks/{taskId}/comments")]
public class CommentController : ControllerBase
{
    private static readonly Action<ILogger, int, Exception?> LogCommentNotFound =
        LoggerMessage.Define<int>(LogLevel.Warning, new EventId(1, "CommentNotFound"), "Comment {CommentId} not found.");

    private readonly ICommentDatabaseService service;
    private readonly ILogger<CommentController> logger;

    public CommentController(ICommentDatabaseService service, ILogger<CommentController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetByTaskId(int taskId)
    {
        var comments = await this.service.GetByTaskIdAsync(taskId);
        return this.Ok(comments.Select(c => new CommentModel
        {
            Id = c.Id,
            Text = c.Text,
            CreatedAt = c.CreatedAt,
            TaskId = c.TaskId,
        }));
    }

    [HttpPost]
    public async Task<ActionResult<CommentModel>> Create(int taskId, [FromBody] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return this.BadRequest("Text cannot be empty.");
        }

        var comment = await this.service.CreateAsync(taskId, text);
        return this.CreatedAtAction(nameof(this.GetByTaskId), new { taskId }, new CommentModel
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            TaskId = comment.TaskId,
        });
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> Update(int taskId, int commentId, [FromBody] string text)
    {
        _ = taskId;
        if (string.IsNullOrWhiteSpace(text))
        {
            return this.BadRequest("Text cannot be empty.");
        }

        try
        {
            await this.service.UpdateAsync(commentId, text);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogCommentNotFound(this.logger, commentId, ex);
            return this.NotFound();
        }
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete(int taskId, int commentId)
    {
        _ = taskId;
        try
        {
            await this.service.DeleteAsync(commentId);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogCommentNotFound(this.logger, commentId, ex);
            return this.NotFound();
        }
    }
}
