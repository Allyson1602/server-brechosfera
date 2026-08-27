using Brechosfera.Domain.Enums;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Requests;

public sealed record StoreRequest(
    Guid OwnerUserId,
    string Name,
    string ResponsibleName,
    decimal AveragePrice,
    ELocalType LocalType,
    IEnumerable<string> ProductTypeKeys,
    int? AverageQuantity,
    string? Description,
    AddressRequest? Address,
    List<SocialMedia>? socialMedia,
    IEnumerable<Phone>? phones
);
