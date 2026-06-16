using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;

public class CategoryModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
