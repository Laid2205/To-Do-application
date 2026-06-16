namespace TodoListApp.WebApi.Services;

public interface ITagDatabaseService
{
    Task<IEnumerable<Tag>> GetAllAsync();

    Task<IEnumerable<Tag>> GetByTaskIdAsync(int taskId);

    Task<IEnumerable<TodoTask>> GetTasksByTagAsync(string label);

    Task<Tag> AddTagToTaskAsync(int taskId, string label);

    Task RemoveTagFromTaskAsync(int taskId, int tagId);
}
