namespace TodoListApp.DataAccess.Entities;

public class CategoryEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
