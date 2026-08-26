using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;
public interface IUserCredentialRepository : IRepository<UserCredential>
{
    Task<UserCredential?> GetByLoginAsync(string usernameOrEmail, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
