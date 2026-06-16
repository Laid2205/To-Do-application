using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentWebApiService service;

    public CommentController(ICommentWebApiService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int taskId, int todoListId, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            await this.service.CreateAsync(taskId, text);
        }

        return this.RedirectToAction("Details", "Task", new { todoListId, id = taskId });
    }

    public async Task<IActionResult> Edit(int taskId, int todoListId, int commentId)
    {
        var comments = await this.service.GetByTaskIdAsync(taskId);
        var comment = comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            return this.NotFound();
        }

        return this.View(new CommentWebApiModel
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            TaskId = taskId,
            TodoListId = todoListId,
        });
    }

    [HttpPost]
    [ActionName("Edit")]
    public async Task<IActionResult> EditConfirmed(int taskId, int todoListId, int commentId, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            await this.service.UpdateAsync(taskId, commentId, text);
        }

        return this.RedirectToAction("Details", "Task", new { todoListId, id = taskId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int taskId, int todoListId, int commentId)
    {
        await this.service.DeleteAsync(taskId, commentId);
        return this.RedirectToAction("Details", "Task", new { todoListId, id = taskId });
    }
}
