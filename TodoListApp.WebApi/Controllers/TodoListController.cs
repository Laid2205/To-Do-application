using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoListController : ControllerBase
{
    private static readonly Action<ILogger, int, Exception?> LogTodoListNotFound =
        LoggerMessage.Define<int>(LogLevel.Warning, new EventId(1, "TodoListNotFound"), "TodoList with id {Id} not found.");

    private readonly ITodoListDatabaseService service;
    private readonly ILogger<TodoListController> logger;

    public TodoListController(ITodoListDatabaseService service, ILogger<TodoListController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListModel>>> GetAll()
    {
        var items = await this.service.GetAllAsync();
        return this.Ok(items.Select(t => new TodoListModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListModel>> GetById(int id)
    {
        var item = await this.service.GetByIdAsync(id);
        if (item == null)
        {
            return this.NotFound();
        }

        return this.Ok(new TodoListModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
        });
    }

    [HttpPost]
    public async Task<ActionResult<TodoListModel>> Create([FromBody] TodoListModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Model cannot be null.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var created = await this.service.CreateAsync(new TodoList
        {
            Title = model.Title,
            Description = model.Description,
        });

        model.Id = created.Id;
        return this.CreatedAtAction(nameof(this.GetAll), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TodoListModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Model cannot be null.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            await this.service.UpdateAsync(id, new TodoList
            {
                Title = model.Title,
                Description = model.Description,
            });

            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTodoListNotFound(this.logger, id, ex);
            return this.NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await this.service.DeleteAsync(id);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTodoListNotFound(this.logger, id, ex);
            return this.NotFound();
        }
    }
}
