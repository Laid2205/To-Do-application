using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.DataAccess.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IJwtTokenService jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtTokenService = jwtTokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseModel>> Register([FromBody] RegisterModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return this.BadRequest(result.Errors.Select(e => e.Description));
        }

        return this.Ok(new AuthResponseModel
        {
            Token = this.jwtTokenService.CreateToken(user),
            Email = user.Email ?? string.Empty,
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseModel>> Login([FromBody] LoginModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return this.Unauthorized("Invalid login attempt.");
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return this.Unauthorized("Invalid login attempt.");
        }

        return this.Ok(new AuthResponseModel
        {
            Token = this.jwtTokenService.CreateToken(user),
            Email = user.Email ?? string.Empty,
        });
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        return this.Ok(new
        {
            Email = this.User.Identity?.Name ?? this.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
        });
    }
}
