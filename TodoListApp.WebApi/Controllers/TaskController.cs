using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/todolists/{todoListId}/tasks")]
public class TaskController : ControllerBase
{
    private static readonly Action<ILogger, int, Exception?> LogTaskNotFound =
        LoggerMessage.Define<int>(LogLevel.Warning, new EventId(1, "TaskNotFound"), "Task with id {Id} not found.");

    private readonly ITaskDatabaseService service;
    private readonly ILogger<TaskController> logger;

    public TaskController(ITaskDatabaseService service, ILogger<TaskController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultModel<TodoTaskModel>>> GetAll(
        int todoListId,
        [FromQuery] string? searchText,
        [FromQuery] string? category,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var tasks = await this.service.GetFilteredAsync(todoListId, searchText, category, pageNumber, pageSize);
        return this.Ok(new PagedResultModel<TodoTaskModel>
        {
            Items = tasks.Items.Select(t => MapToModel(t)),
            PageNumber = tasks.PageNumber,
            PageSize = tasks.PageSize,
            TotalCount = tasks.TotalCount,
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoTaskModel>> GetById(int todoListId, int id)
    {
        var task = await this.service.GetByIdAsync(id);
        if (task == null || task.TodoListId != todoListId)
        {
            return this.NotFound();
        }

        return this.Ok(MapToModel(task));
    }

    [HttpPost]
    public async Task<ActionResult<TodoTaskModel>> Create(int todoListId, [FromBody] TodoTaskModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Model cannot be null.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        model.TodoListId = todoListId;
        var created = await this.service.CreateAsync(MapToService(model));
        model.Id = created.Id;
        model.CreatedAt = created.CreatedAt;

        return this.CreatedAtAction(nameof(this.GetById), new { todoListId, id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int todoListId, int id, [FromBody] TodoTaskModel model)
    {
        if (model == null)
        {
            return this.BadRequest("Model cannot be null.");
        }

        try
        {
            model.TodoListId = todoListId;
            await this.service.UpdateAsync(id, MapToService(model));
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTaskNotFound(this.logger, id, ex);
            return this.NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int todoListId, int id)
    {
        try
        {
            var task = await this.service.GetByIdAsync(id);
            if (task == null || task.TodoListId != todoListId)
            {
                return this.NotFound();
            }

            await this.service.DeleteAsync(id);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogTaskNotFound(this.logger, id, ex);
            return this.NotFound();
        }
    }

    private static TodoTaskModel MapToModel(TodoTask task)
    {
        return new TodoTaskModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            TodoListId = task.TodoListId,
            CategoryId = task.CategoryId,
            CategoryName = task.CategoryName,
        };
    }

    private static TodoTask MapToService(TodoTaskModel model)
    {
        return new TodoTask
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Status = model.Status,
            AssigneeId = model.AssigneeId,
            TodoListId = model.TodoListId,
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
        };
    }
}
