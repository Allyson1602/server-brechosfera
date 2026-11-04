using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Tests.Common.Factories;

public static class AddressFactory
{
    public static Address Create(
        string cep = "01001000",
        string street = "Praça da Sé",
        string neighborhood = "Sé",
        string city = "São Paulo",
        string stateCode = "SP",
        string state = "São Paulo",
        string? complement = null
    ) => new(
        cep: cep,
        street: street,
        neighborhood: neighborhood,
        city: city,
        stateCode: stateCode,
        state: state,
        complement: complement
    );
}
