using Microsoft.Extensions.DependencyInjection;
using TodoListApp.DataAccess.Repositories;

namespace TodoListApp.DataAccess;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        return services;
    }
}
