using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskWebApiService service;
    private readonly ITagWebApiService tagService;
    private readonly ICommentWebApiService commentService;

    public TaskController(
        ITaskWebApiService service,
        ITagWebApiService tagService,
        ICommentWebApiService commentService)
    {
        this.service = service;
        this.tagService = tagService;
        this.commentService = commentService;
    }

    public async Task<IActionResult> Index(
        int todoListId,
        string? searchText,
        string? category,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var tasks = await this.service.GetFilteredAsync(todoListId, searchText, category, pageNumber, pageSize);
        var tags = await this.tagService.GetAllAsync();

        this.ViewBag.TodoListId = todoListId;
        this.ViewBag.SearchText = searchText;
        this.ViewBag.Category = category;
        this.ViewBag.PageNumber = tasks.PageNumber;
        this.ViewBag.PageSize = tasks.PageSize;
        this.ViewBag.TotalPages = tasks.TotalPages;
        this.ViewBag.TotalCount = tasks.TotalCount;
        this.ViewBag.Categories = tags
            .Select(t => t.Label)
            .Where(label => !string.IsNullOrWhiteSpace(label))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(label => label)
            .ToList();

        return this.View(tasks.Items.Select(t => MapToModel(t)));
    }

    public async Task<IActionResult> Details(int todoListId, int id)
    {
        var task = await this.service.GetByIdAsync(todoListId, id);
        if (task == null)
        {
            return this.NotFound();
        }

        var model = MapToModel(task);

        var tags = await this.tagService.GetByTaskIdAsync(id);
        foreach (var t in tags)
        {
            model.Tags.Add(new TagWebApiModel { Id = t.Id, Label = t.Label, TaskId = t.TaskId });
        }

        var comments = await this.commentService.GetByTaskIdAsync(id);
        foreach (var c in comments)
        {
            model.Comments.Add(new CommentWebApiModel
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                TaskId = c.TaskId,
            });
        }

        return this.View(model);
    }

    public IActionResult Create(int todoListId)
    {
        this.ViewBag.TodoListId = todoListId;
        return this.View(new TodoTaskWebApiModel { TodoListId = todoListId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoTaskWebApiModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        this.ModelState.Remove("AssigneeId");
        this.ModelState.Remove("Tags");
        this.ModelState.Remove("Comments");

        if (!this.ModelState.IsValid)
        {
            this.ViewBag.TodoListId = model.TodoListId;
            return this.View(model);
        }

        var data = MapToData(model);
        data.AssigneeId = this.User.Identity?.Name ?? string.Empty;

        await this.service.CreateAsync(data);
        return this.RedirectToAction(nameof(this.Index), new { todoListId = model.TodoListId });
    }

    public async Task<IActionResult> Edit(int todoListId, int id)
    {
        return await this.GetTaskViewAsync(todoListId, id);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int todoListId, int id, TodoTaskWebApiModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var data = MapToData(model);
        data.AssigneeId = this.User.Identity?.Name ?? string.Empty;

        await this.service.UpdateAsync(todoListId, id, data);
        return this.RedirectToAction(nameof(this.Index), new { todoListId });
    }

    public Task<IActionResult> Delete(int todoListId, int id)
    {
        return this.GetTaskViewAsync(todoListId, id);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int todoListId, int id)
    {
        await this.service.DeleteAsync(todoListId, id);
        return this.RedirectToAction(nameof(this.Index), new { todoListId });
    }

    private static TodoTaskWebApiModel MapToModel(TodoTaskData data)
    {
        return new TodoTaskWebApiModel
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            CreatedAt = data.CreatedAt,
            DueDate = data.DueDate,
            Status = data.Status,
            AssigneeId = data.AssigneeId,
            TodoListId = data.TodoListId,
        };
    }

    private static TodoTaskData MapToData(TodoTaskWebApiModel model)
    {
        return new TodoTaskData
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Status = model.Status,
            AssigneeId = model.AssigneeId,
            TodoListId = model.TodoListId,
        };
    }

    private async Task<IActionResult> GetTaskViewAsync(int todoListId, int id)
    {
        var task = await this.service.GetByIdAsync(todoListId, id);
        if (task == null)
        {
            return this.NotFound();
        }

        return this.View(MapToModel(task));
    }
}
