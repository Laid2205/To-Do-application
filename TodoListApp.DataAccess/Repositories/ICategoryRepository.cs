using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryEntity>> GetAllAsync();

    Task<CategoryEntity?> GetByIdAsync(int id);

    Task<CategoryEntity?> GetByNameAsync(string name);

    Task<CategoryEntity> CreateAsync(CategoryEntity entity);

    Task UpdateAsync(CategoryEntity entity);

    Task DeleteAsync(int id);
}
