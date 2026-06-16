using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<CommentEntity>> GetByTaskIdAsync(int taskId);

    Task<CommentEntity?> GetByIdAsync(int id);

    Task<CommentEntity> CreateAsync(CommentEntity entity);

    Task UpdateAsync(CommentEntity entity);

    Task DeleteAsync(int id);
}
