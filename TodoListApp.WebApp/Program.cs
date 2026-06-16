using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDb")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<UsersDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

var webApiBaseUrl = builder.Configuration["WebApi:BaseUrl"]
    ?? throw new InvalidOperationException("WebApi:BaseUrl is not configured in appsettings.");

builder.Services.AddTransient<WebApiAuthDelegatingHandler>();
builder.Services.AddHttpClient("WebApi", client =>
{
    client.BaseAddress = new Uri(webApiBaseUrl);
}).AddHttpMessageHandler<WebApiAuthDelegatingHandler>();

builder.Services.AddScoped<IWebApiAuthService, WebApiAuthService>();

builder.Services.AddScoped<ITodoListWebApiService, TodoListWebApiService>();
builder.Services.AddScoped<ITaskWebApiService, TaskWebApiService>();
builder.Services.AddScoped<IAssignedTaskWebApiService, AssignedTaskWebApiService>();
builder.Services.AddScoped<ITagWebApiService, TagWebApiService>();
builder.Services.AddScoped<ICommentWebApiService, CommentWebApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TodoList}/{action=Index}/{id?}");

app.Run();
