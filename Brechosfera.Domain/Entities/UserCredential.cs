using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Guards;

namespace Brechosfera.Domain.Entities;

public class UserCredential : Entity, IAggregateRoot
{
    private readonly List<ERole> _roles = new();

    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsEmailConfirmed { get; private set; } = false;
    public IReadOnlyCollection<ERole> Roles => _roles.AsReadOnly();
    //public Guid RefreshToken { get; private set; }

    private UserCredential() { }

    public UserCredential(string username, string email, string passwordHash)
    {
        Guard.AgainstNullOrWhiteSpace(username, "CREDENTIAL_NAME_NOT_FOUND", "O nome do usuário é obrigatório.");

        string cleanedUsername = username.Trim().ToLower();

        Guard.AgainstRange(cleanedUsername, 3, 30, "CREDENTIAL_NAME_OUT_LENGTH", "O nome de usuário deve ter entre 3 e 30 caracteres.");
        Guard.AgainstSpaces(cleanedUsername, "CREDENTIAL_NAME_WITH_SPACE", "O nome de usuário não pode conter espaços.");

        Guard.AgainstNullOrWhiteSpace(email, "CREDENTIAL_EMAIL_NOT_FOUND", "O e-mail é obrigatório.");

        Guard.AgainstNullOrWhiteSpace(passwordHash, "CREDENTIAL_PASSWORD_NOT_FOUND", "A senha é obrigatória.");
        Guard.AgainstRange(passwordHash, 40, 100, "CREDENTIAL_PASSWORD_INVALID_SIZE", "A senha possui um tamanho inválido.");

        Username = cleanedUsername;
        Email = email.Trim().ToLower();
        PasswordHash = passwordHash;
    }

    public void UpdateUsername(string username)
    {
        Guard.AgainstNullOrWhiteSpace(username, "CREDENTIAL_NAME_NOT_FOUND", "O nome do usuário é obrigatório.");

        string cleanedUsername = username.Trim().ToLower();

        Guard.AgainstRange(cleanedUsername, 3, 30, "CREDENTIAL_NAME_OUT_LENGTH", "O nome de usuário deve ter entre 3 e 30 caracteres.");
        Guard.AgainstSpaces(cleanedUsername, "CREDENTIAL_NAME_WITH_SPACE", "O nome de usuário não pode conter espaços.");


        Username = cleanedUsername;
    }

    public void UpdateEmail(string email)
    {
        Guard.AgainstNullOrWhiteSpace(email, "CREDENTIAL_EMAIL_NOT_FOUND", "O e-mail é obrigatório.");

        Email = email.Trim().ToLower();
    }

    public void UpdatePassword(string passwordHash)
    {
        Guard.AgainstNullOrWhiteSpace(passwordHash, "CREDENTIAL_PASSWORD_NOT_FOUND", "A senha é obrigatória.");
        Guard.AgainstRange(passwordHash, 40, 100, "CREDENTIAL_PASSWORD_INVALID_SIZE", "A senha possui um tamanho inválido.");

        PasswordHash = passwordHash;
    }

    public void ConfirmEmail()
    {
        if (IsEmailConfirmed) return;

        IsEmailConfirmed = true;
    }

    public void InvalidateEmail()
    {
        if (!IsEmailConfirmed) return;

        IsEmailConfirmed = false;
    }

    public void AddRole(ERole role)
    {
        if (_roles.Contains(role)) return;

        _roles.Add(role);
    }

    public void RemoveRole(ERole role)
    {
        if (!_roles.Contains(role)) return;

        _roles.Remove(role);
    }
}
