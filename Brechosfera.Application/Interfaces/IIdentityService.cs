using Brechosfera.Application.Common;
using Brechosfera.Application.Requests;

namespace Brechosfera.Application.Interfaces;

public interface IIdentityService
{
    Task<(Result Result, string? UserId)> RegisterAsync(RegisterUserRequest request);
    Task<bool> UserNameExistsAsync(string userName);
}
