using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskSearchController : ControllerBase
{
    private readonly ITaskDatabaseService service;

    public TaskSearchController(ITaskDatabaseService service)
    {
        this.service = service;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TodoTaskModel>>> Search([FromQuery] string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return this.BadRequest("Search text cannot be empty.");
        }

        var tasks = await this.service.SearchByTitleAsync(searchText);
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
}
