using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class AssignedTaskController : Controller
{
    private readonly IAssignedTaskWebApiService service;

    public AssignedTaskController(IAssignedTaskWebApiService service)
    {
        this.service = service;
    }

    public async Task<IActionResult> Index(
        string? statusFilter = null,
        string? sortBy = null)
    {
        var assigneeId = this.User.Identity?.Name ?? string.Empty;
        var tasks = await this.service.GetAssignedTasksAsync(assigneeId, statusFilter, sortBy);

        this.ViewBag.AssigneeId = assigneeId;
        this.ViewBag.StatusFilter = statusFilter;
        this.ViewBag.SortBy = sortBy;

        return this.View(tasks.Select(t => new AssignedTaskWebApiModel
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
    public async Task<IActionResult> UpdateStatus(int taskId, TodoTaskStatus newStatus)
    {
        var assigneeId = this.User.Identity?.Name ?? string.Empty;
        await this.service.UpdateStatusAsync(assigneeId, taskId, newStatus);
        return this.RedirectToAction(nameof(this.Index));
    }
}
