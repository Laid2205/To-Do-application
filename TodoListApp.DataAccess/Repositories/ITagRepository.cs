using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<TagEntity>> GetAllAsync();

    Task<IEnumerable<TagEntity>> GetByTaskIdAsync(int taskId);

    Task<IEnumerable<TaskEntity>> GetTasksByTagAsync(string label);

    Task<TagEntity> AddTagToTaskAsync(int taskId, string label);

    Task RemoveTagFromTaskAsync(int taskId, int tagId);
}
