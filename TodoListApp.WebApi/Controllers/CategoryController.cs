using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private static readonly Action<ILogger, int, Exception?> LogCategoryNotFound =
        LoggerMessage.Define<int>(LogLevel.Warning, new EventId(1, "CategoryNotFound"), "Category with id {Id} not found.");

    private readonly ICategoryDatabaseService service;
    private readonly ILogger<CategoryController> logger;

    public CategoryController(ICategoryDatabaseService service, ILogger<CategoryController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryModel>>> GetAll()
    {
        var categories = await this.service.GetAllAsync();
        return this.Ok(categories.Select(c => new CategoryModel { Id = c.Id, Name = c.Name }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryModel>> GetById(int id)
    {
        var category = await this.service.GetByIdAsync(id);
        if (category == null)
        {
            return this.NotFound();
        }

        return this.Ok(new CategoryModel { Id = category.Id, Name = category.Name });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryModel>> Create([FromBody] CategoryModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var created = await this.service.CreateAsync(new Category { Name = model.Name });
        model.Id = created.Id;
        return this.CreatedAtAction(nameof(this.GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            await this.service.UpdateAsync(id, new Category { Name = model.Name });
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            LogCategoryNotFound(this.logger, id, ex);
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
            LogCategoryNotFound(this.logger, id, ex);
            return this.NotFound();
        }
    }
}
