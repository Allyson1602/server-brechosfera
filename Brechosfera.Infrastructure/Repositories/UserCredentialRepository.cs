using Brechosfera.Domain.Repositories;
using Brechosfera.Domain.Entities;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class UserCredentialRepository : Repository<UserCredential>, IUserCredentialRepository
{
    public UserCredentialRepository(AppDbContext context) : base(context) { }

    public async Task<UserCredential?> GetByLoginAsync(string usernameOrEmail, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == usernameOrEmail || u.Username == usernameOrEmail, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
