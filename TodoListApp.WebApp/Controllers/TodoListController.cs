using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TodoListController : Controller
{
    private static readonly Action<ILogger, Exception?> LogApiError =
        LoggerMessage.Define(LogLevel.Error, new EventId(1, "ApiError"), "Error connecting to WebApi.");

    private readonly ITodoListWebApiService service;
    private readonly ILogger<TodoListController> logger;

    public TodoListController(ITodoListWebApiService service, ILogger<TodoListController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var items = await this.service.GetAllAsync();
            return this.View(items.Select(t => new TodoListWebApiModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
            }));
        }
        catch (HttpRequestException ex)
        {
            LogApiError(this.logger, ex);
            return this.View(Enumerable.Empty<TodoListWebApiModel>());
        }
    }

    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        await this.service.CreateAsync(new TodoListData
        {
            Title = model.Title,
            Description = model.Description,
        });

        return this.RedirectToAction(nameof(this.Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        return await this.GetItemViewAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        await this.service.UpdateAsync(id, new TodoListData
        {
            Title = model.Title,
            Description = model.Description,
        });

        return this.RedirectToAction(nameof(this.Index));
    }

    public Task<IActionResult> Delete(int id)
    {
        return this.GetItemViewAsync(id);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await this.service.DeleteAsync(id);
        return this.RedirectToAction(nameof(this.Index));
    }

    private async Task<IActionResult> GetItemViewAsync(int id)
    {
        var item = await this.service.GetByIdAsync(id);
        if (item == null)
        {
            return this.NotFound();
        }

        return this.View(new TodoListWebApiModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
        });
    }
}
