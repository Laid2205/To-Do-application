namespace TodoListApp.WebApi.Services;

public interface ICategoryDatabaseService
{
    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> CreateAsync(Category category);

    Task UpdateAsync(int id, Category category);

    Task DeleteAsync(int id);
}
