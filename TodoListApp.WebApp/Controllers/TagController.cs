using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TagController : Controller
{
    private readonly ITagWebApiService service;

    public TagController(ITagWebApiService service)
    {
        this.service = service;
    }

    public async Task<IActionResult> Index()
    {
        var tags = await this.service.GetAllAsync();
        return this.View(tags.Select(t => new TagWebApiModel
        {
            Id = t.Id,
            Label = t.Label,
            TaskId = t.TaskId,
        }));
    }

    public async Task<IActionResult> TasksByTag(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        var tasks = await this.service.GetTasksByTagAsync(label);
        this.ViewBag.Label = label;

        return this.View(tasks.Select(t => new TodoTaskWebApiModel
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

    [HttpPost]
    public async Task<IActionResult> AddTag(int taskId, int todoListId, string label)
    {
        if (!string.IsNullOrWhiteSpace(label))
        {
            await this.service.AddTagToTaskAsync(taskId, label);
        }

        return this.RedirectToAction("Details", "Task", new { todoListId, id = taskId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveTag(int taskId, int todoListId, int tagId)
    {
        await this.service.RemoveTagFromTaskAsync(taskId, tagId);
        return this.RedirectToAction("Details", "Task", new { todoListId, id = taskId });
    }
}
