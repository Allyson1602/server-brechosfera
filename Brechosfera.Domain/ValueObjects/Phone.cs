using Brechosfera.Domain.Guards;

namespace Brechosfera.Domain.ValueObjects;

public sealed record Phone
{
    public string Number { get; init; } = string.Empty;
    public bool IsPrimary { get; set; }

    private Phone() { }

    public Phone(string number, bool isPrimary = false)
    {
        Guard.AgainstNullOrWhiteSpace(number, "PHONE_NOT_FOUND", "O número do celular é obrigatório.");
        Guard.AgainstNonDigits(number, "PHONE_ONLY_NUMBERS", "O número do celular deve conter apenas dígitos numéricos.");

        Number = number;
        IsPrimary = isPrimary;
    }
}
