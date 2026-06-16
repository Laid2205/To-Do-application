using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.DataAccess.Entities;

namespace TodoListApp.DataAccess.Data;

public class TodoListDbContext : IdentityDbContext<ApplicationUser>
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; } = null!;

    public DbSet<TaskEntity> Tasks { get; set; } = null!;

    public DbSet<CategoryEntity> Categories { get; set; } = null!;

    public DbSet<TagEntity> Tags { get; set; } = null!;

    public DbSet<CommentEntity> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CategoryEntity>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<TaskEntity>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
