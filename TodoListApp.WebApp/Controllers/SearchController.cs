using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly ITaskWebApiService service;

    public SearchController(ITaskWebApiService service)
    {
        this.service = service;
    }

    public IActionResult Index()
    {
        return this.View();
    }

    [HttpGet]
    public async Task<IActionResult> Results([FromQuery] string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        var tasks = await this.service.SearchByTitleAsync(searchText);
        this.ViewBag.SearchText = searchText;

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
}
