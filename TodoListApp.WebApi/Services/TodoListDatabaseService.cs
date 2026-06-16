using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly ITodoListRepository todoListRepository;

    public TodoListDatabaseService(ITodoListRepository todoListRepository)
    {
        this.todoListRepository = todoListRepository;
    }

    public async Task<IEnumerable<TodoList>> GetAllAsync()
    {
        var entities = await this.todoListRepository.GetAllAsync();
        return entities.Select(e => new TodoList
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
        });
    }

    public async Task<TodoList?> GetByIdAsync(int id)
    {
        var entity = await this.todoListRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return null;
        }

        return new TodoList { Id = entity.Id, Title = entity.Title, Description = entity.Description };
    }

    public Task<TodoList> CreateAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        return this.CreateAsyncCore(todoList);
    }

    public Task UpdateAsync(int id, TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        return this.UpdateAsyncCore(id, todoList);
    }

    public Task DeleteAsync(int id)
    {
        return this.todoListRepository.DeleteAsync(id);
    }

    private async Task<TodoList> CreateAsyncCore(TodoList todoList)
    {
        var entity = new TodoListEntity
        {
            Title = todoList.Title,
            Description = todoList.Description,
        };

        var created = await this.todoListRepository.CreateAsync(entity);
        todoList.Id = created.Id;
        return todoList;
    }

    private async Task UpdateAsyncCore(int id, TodoList todoList)
    {
        var entity = await this.todoListRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"TodoList with id {id} not found.");

        entity.Title = todoList.Title;
        entity.Description = todoList.Description;

        await this.todoListRepository.UpdateAsync(entity);
    }
}
