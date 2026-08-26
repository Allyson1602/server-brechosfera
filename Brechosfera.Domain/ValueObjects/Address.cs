using Brechosfera.Domain.Guards;

namespace Brechosfera.Domain.ValueObjects;

public sealed record Address
{
    public string Cep { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string Neighborhood { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string StateCode { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string? Complement { get; private set; } = string.Empty;

    protected Address() { }

    public Address(
        string cep,
        string street,
        string neighborhood,
        string city,
        string stateCode,
        string state,
        string? complement
    )
    {
        Guard.AgainstNullOrWhiteSpace(cep, "ADDRESS_INVALID_CEP", "CEP inválido.");
        Guard.AgainstNullOrWhiteSpace(street, "ADDRESS_INVALID_STREET", "Rua inválida.");
        Guard.AgainstNullOrWhiteSpace(neighborhood, "ADDRESS_INVALID_NEIGHBORHOOD", "Bairro inválido.");
        Guard.AgainstNullOrWhiteSpace(city, "ADDRESS_INVALID_CITY", "Cidade inválida.");
        Guard.AgainstNullOrWhiteSpace(stateCode, "ADDRESS_INVALID_UF", "Unidade federativa inválida.");
        Guard.AgainstNullOrWhiteSpace(state, "ADDRESS_INVALID_STATE", "Estado inválido.");

        Cep = cep;
        Street = street;
        Neighborhood = neighborhood;
        City = city;
        StateCode = stateCode;
        State = state;
        Complement = complement;
    }
}