using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Requests;
using Microsoft.AspNetCore.Identity;

namespace Brechosfera.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public async Task<(Result, string?)> RegisterAsync(RegisterUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        return result.Succeeded
            ? (Result.Success(), user.Id.ToString())
            : (Result.Failure(result.Errors.Select(e => e.Description)), null);
    }

    public async Task<bool> UserNameExistsAsync(string userName)
        => await _userManager.FindByNameAsync(userName) is not null;
}
