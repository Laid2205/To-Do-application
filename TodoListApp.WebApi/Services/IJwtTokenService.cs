using TodoListApp.DataAccess.Entities;

namespace TodoListApp.WebApi.Services;

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user);
}
