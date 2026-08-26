namespace Brechosfera.Domain.Entities;

public class UserCredential
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public List<ERole> ERoles { get, private set; } = new();
    //public Guid RefreshToken { get, private set; }
}