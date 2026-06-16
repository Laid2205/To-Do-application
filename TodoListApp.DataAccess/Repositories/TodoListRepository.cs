using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Data;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly TodoListDbContext context;

    public TodoListRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TodoListEntity>> GetAllAsync()
    {
        return await this.context.TodoLists.AsNoTracking().ToListAsync();
    }

    public async Task<TodoListEntity?> GetByIdAsync(int id)
    {
        return await this.context.TodoLists.FindAsync(id);
    }

    public async Task<TodoListEntity> CreateAsync(TodoListEntity entity)
    {
        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TodoListEntity entity)
    {
        this.context.TodoLists.Update(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await this.context.TodoLists.FindAsync(id)
            ?? throw new KeyNotFoundException($"TodoList with id {id} not found.");

        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();
    }
}
