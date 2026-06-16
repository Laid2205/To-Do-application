namespace TodoListApp.WebApp.Services;

public interface ITagWebApiService
{
    Task<IEnumerable<TagData>> GetAllAsync();

    Task<IEnumerable<TagData>> GetByTaskIdAsync(int taskId);

    Task<IEnumerable<TodoTaskData>> GetTasksByTagAsync(string label);

    Task AddTagToTaskAsync(int taskId, string label);

    Task RemoveTagFromTaskAsync(int taskId, int tagId);
}

public class TagData
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public int TaskId { get; set; }
}
