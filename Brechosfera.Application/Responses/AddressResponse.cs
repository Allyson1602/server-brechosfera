namespace Brechosfera.Application.Responses;

public sealed record AddressResponse(
     string Cep,
     string Street,
     string Neighborhood,
     string City,
     string StateCode,
     string State,
     string? Complement
);