using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record UpdateStoreRequest(
    string Name,
    decimal AveragePrice,
    ELocalType LocalType,
    int? AverageQuantity,
    string? Description,
    AddressRequest Address
);
