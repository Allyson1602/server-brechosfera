using Brechosfera.Application.Common;
using Brechosfera.Application.Requests;

namespace Brechosfera.Application.Interfaces;

public interface IIdentityService
{
    Task<(Result Result, AuthUser? User)> RegisterAsync(RegisterUserRequest request);
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
    Task<Result> ConfirmEmailAsync(Guid userId, string token);
    Task<(Result Result, AuthUser? User)> CheckCredentialsAsync(string login, string password);
}
