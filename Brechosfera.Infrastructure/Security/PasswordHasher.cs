using Brechosfera.Application.Interfaces;

namespace Brechosfera.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string passwordHash, string providedPassword) => BCrypt.Net.BCrypt.Verify(providedPassword, passwordHash);
}
