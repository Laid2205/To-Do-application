using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public interface ITodoListRepository
{
    Task<IEnumerable<TodoListEntity>> GetAllAsync();

    Task<TodoListEntity?> GetByIdAsync(int id);

    Task<TodoListEntity> CreateAsync(TodoListEntity entity);

    Task UpdateAsync(TodoListEntity entity);

    Task DeleteAsync(int id);
}
