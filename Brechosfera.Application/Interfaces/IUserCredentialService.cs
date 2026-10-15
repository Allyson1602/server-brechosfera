using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IUserCredentialService
{
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateUsername(Guid id, UpdateUsernameRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateEmail(Guid id, UpdateEmailRequest request, CancellationToken cancellationToken);
    Task<bool> UpdatePassword(Guid id, UpdatePasswordRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> RegisterAsync(CreateRegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> LoginAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
}
