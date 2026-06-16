using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly IWebApiAuthService webApiAuthService;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IWebApiAuthService webApiAuthService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.webApiAuthService = webApiAuthService;
    }

    public IActionResult Register()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await this.userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await this.signInManager.SignInAsync(user, isPersistent: false);
            await this.StoreWebApiTokenAsync(model.Email, model.Password, register: true);
            return this.RedirectToAction("Index", "TodoList");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        return this.View(model);
    }

    public IActionResult Login()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (model == null)
        {
            return this.BadRequest();
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            await this.StoreWebApiTokenAsync(model.Email, model.Password, register: false);
            return this.RedirectToAction("Index", "TodoList");
        }

        this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return this.View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        this.HttpContext.Session.Remove("WebApiToken");
        await this.signInManager.SignOutAsync();
        return this.RedirectToAction("Login");
    }

    private async Task StoreWebApiTokenAsync(string email, string password, bool register)
    {
        var token = register
            ? await this.webApiAuthService.RegisterAsync(email, password)
            : await this.webApiAuthService.LoginAsync(email, password);

        if (!string.IsNullOrWhiteSpace(token))
        {
            this.HttpContext.Session.SetString("WebApiToken", token);
        }
    }

    public IActionResult ForgotPassword()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            this.ModelState.AddModelError(string.Empty, "Email is required.");
            return this.View();
        }

        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            this.ModelState.AddModelError(string.Empty, "User with this email not found.");
            return this.View();
        }

        var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = this.Url.Action(
            "ResetPassword",
            "Account",
            new { token, email = user.Email },
            this.Request.Scheme);

        this.ViewBag.ResetLink = resetLink;
        return this.View();
    }

    public IActionResult ResetPassword(string token, string email)
    {
        this.ViewBag.Token = token;
        this.ViewBag.Email = email;
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string token, string email, string newPassword)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return this.RedirectToAction(nameof(this.Login));
        }

        var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            return this.RedirectToAction(nameof(this.Login));
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        this.ViewBag.Token = token;
        this.ViewBag.Email = email;
        return this.View();
    }
}
