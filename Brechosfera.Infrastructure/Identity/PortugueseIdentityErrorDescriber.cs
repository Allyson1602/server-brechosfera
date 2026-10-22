using Microsoft.AspNetCore.Identity;

namespace Brechosfera.Infrastructure.Identity;

public sealed class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName)
        => new() { Code = nameof(DuplicateUserName), Description = $"O nome de usuário '{userName}' já está em uso." };

    public override IdentityError DuplicateEmail(string email)
        => new() { Code = nameof(DuplicateEmail), Description = $"O e-mail '{email}' já está cadastrado." };

    public override IdentityError InvalidUserName(string? userName)
        => new() { Code = nameof(InvalidUserName), Description = "Nome de usuário inválido. Use apenas letras, números, '.', '_' ou '-'." };

    public override IdentityError InvalidEmail(string? email)
        => new() { Code = nameof(InvalidEmail), Description = "E-mail inválido." };

    public override IdentityError PasswordTooShort(int length)
        => new() { Code = nameof(PasswordTooShort), Description = $"A senha deve ter pelo menos {length} caracteres." };

    public override IdentityError PasswordRequiresDigit()
        => new() { Code = nameof(PasswordRequiresDigit), Description = "A senha deve conter pelo menos um número." };

    public override IdentityError PasswordRequiresUpper()
        => new() { Code = nameof(PasswordRequiresUpper), Description = "A senha deve conter pelo menos uma letra maiúscula." };

    public override IdentityError PasswordRequiresLower()
        => new() { Code = nameof(PasswordRequiresLower), Description = "A senha deve conter pelo menos uma letra minúscula." };

    public override IdentityError PasswordMismatch()
        => new() { Code = nameof(PasswordMismatch), Description = "Senha incorreta." };

    public override IdentityError InvalidToken()
        => new() { Code = nameof(InvalidToken), Description = "Token inválido ou expirado." };

    public override IdentityError DefaultError()
        => new() { Code = nameof(DefaultError), Description = "Ocorreu um erro inesperado." };
}
