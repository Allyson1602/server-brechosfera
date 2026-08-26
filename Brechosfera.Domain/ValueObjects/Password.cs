using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.Guards;
using System.Text.RegularExpressions;

namespace Brechosfera.Domain.ValueObjects;

public record Password
{
    public string Value { get; }

    private Password(string value)
    {
        Value = value;
    }

    public static Password Create(string password)
    {
        Guard.AgainstNullOrWhiteSpace(password, "PASSWORD_EMPTY", "A senha não pode estar vazia.");

        if (password.Length < 8)
            throw new DomainException("PASSWORD_MORE_LIMITE", "A senha deve ter pelo menos 8 caracteres.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            throw new DomainException("PASSWORD_WITHOUT_UPPER", "A senha deve conter pelo menos uma letra maiúscula.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            throw new DomainException("PASSWORD_WITHOUT_LOWER", "A senha deve conter pelo menos uma letra minúscula.");

        if (!Regex.IsMatch(password, @"[0-9]"))
            throw new DomainException("PASSWORD_WITHOUT_NUMBER", "A senha deve conter pelo menos um número.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            throw new DomainException("PASSWORD_WITHOUT_CHAR", "A senha deve conter pelo menos um caractere especial.");

        return new Password(password);
    }
}
