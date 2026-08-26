namespace Brechosfera.Application.Requests;

public sealed record AddressRequest (
    string Cep,
    string Street,
    string Neighborhood,
    string City,
    string StateCode,
    string State,
    string? Complement
);
