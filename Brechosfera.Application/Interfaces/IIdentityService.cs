using Brechosfera.Application.Common;
using Brechosfera.Application.Requests;

namespace Brechosfera.Application.Interfaces;

public interface IIdentityService
{
    Task<AuthUser?> FindUnconfirmedByEmailAsync(string email);
    Task<(Result Result, AuthUser? User)> RegisterAsync(RegisterUserRequest request);
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
    Task<(Result Result, AuthUser? NewlyConfirmedUser)> ConfirmEmailAsync(Guid userId, string token);
    Task<(Result Result, AuthUser? User)> CheckCredentialsAsync(string login, string password);
}
