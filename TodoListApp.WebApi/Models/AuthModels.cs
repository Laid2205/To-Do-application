using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseModel
{
    public string Token { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
