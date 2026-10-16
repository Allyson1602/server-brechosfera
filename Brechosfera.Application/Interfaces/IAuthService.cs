using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IAuthService
{
    Task<Guid?> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<bool> ConfirmEmailAsync(Guid userId, string token);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
