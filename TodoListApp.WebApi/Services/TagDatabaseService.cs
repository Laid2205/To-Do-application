using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.WebApi.Services;

public class TagDatabaseService : ITagDatabaseService
{
    private readonly ITagRepository tagRepository;

    public TagDatabaseService(ITagRepository tagRepository)
    {
        this.tagRepository = tagRepository;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        var entities = await this.tagRepository.GetAllAsync();
        return entities.Select(t => new Tag { Id = t.Id, Label = t.Label, TaskId = t.TaskId });
    }

    public async Task<IEnumerable<Tag>> GetByTaskIdAsync(int taskId)
    {
        var entities = await this.tagRepository.GetByTaskIdAsync(taskId);
        return entities.Select(t => new Tag { Id = t.Id, Label = t.Label, TaskId = t.TaskId });
    }

    public Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        return this.GetTasksByTagAsyncCore(label);
    }

    public Task<Tag> AddTagToTaskAsync(int taskId, string label)
    {
        ArgumentNullException.ThrowIfNull(label);
        return this.AddTagToTaskAsyncCore(taskId, label);
    }

    public Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        return this.tagRepository.RemoveTagFromTaskAsync(taskId, tagId);
    }

    private async Task<IEnumerable<TodoTask>> GetTasksByTagAsyncCore(string label)
    {
        var entities = await this.tagRepository.GetTasksByTagAsync(label);
        return entities.Select(t => new TodoTask
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            TodoListId = t.TodoListId,
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name,
        });
    }

    private async Task<Tag> AddTagToTaskAsyncCore(int taskId, string label)
    {
        var entity = await this.tagRepository.AddTagToTaskAsync(taskId, label);
        return new Tag { Id = entity.Id, Label = entity.Label, TaskId = entity.TaskId };
    }
}
