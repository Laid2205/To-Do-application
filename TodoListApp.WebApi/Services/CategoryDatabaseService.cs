using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class CategoryDatabaseService : ICategoryDatabaseService
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryDatabaseService(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var entities = await this.categoryRepository.GetAllAsync();
        return entities.Select(e => new Category { Id = e.Id, Name = e.Name });
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var entity = await this.categoryRepository.GetByIdAsync(id);
        return entity == null ? null : new Category { Id = entity.Id, Name = entity.Name };
    }

    public Task<Category> CreateAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return this.CreateAsyncCore(category);
    }

    public Task UpdateAsync(int id, Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return this.UpdateAsyncCore(id, category);
    }

    public Task DeleteAsync(int id)
    {
        return this.categoryRepository.DeleteAsync(id);
    }

    private async Task<Category> CreateAsyncCore(Category category)
    {
        var entity = new CategoryEntity { Name = category.Name.Trim() };
        var created = await this.categoryRepository.CreateAsync(entity);
        category.Id = created.Id;
        return category;
    }

    private async Task UpdateAsyncCore(int id, Category category)
    {
        var entity = await this.categoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        entity.Name = category.Name.Trim();
        await this.categoryRepository.UpdateAsync(entity);
    }
}
