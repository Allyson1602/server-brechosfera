using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IUserCredentialService
{
    Task<UserResponse?> RegisterAsync(CreateRegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> LoginAsync(CreateLoginRequest request, CancellationToken cancellationToken = default);
}
