using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Data;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly TodoListDbContext context;

    public CategoryRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<CategoryEntity>> GetAllAsync()
    {
        return await this.context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CategoryEntity?> GetByIdAsync(int id)
    {
        return await this.context.Categories.FindAsync(id);
    }

    public async Task<CategoryEntity?> GetByNameAsync(string name)
    {
        return await this.context.Categories
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<CategoryEntity> CreateAsync(CategoryEntity entity)
    {
        this.context.Categories.Add(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(CategoryEntity entity)
    {
        this.context.Categories.Update(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Category with id {id} not found.");

        this.context.Categories.Remove(entity);
        await this.context.SaveChangesAsync();
    }
}
