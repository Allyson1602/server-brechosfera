using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record CreateStoreRequest(
    Guid OwnerUserId,
    string Name,
    decimal AveragePrice,
    ELocalType LocalType,
    IEnumerable<string> ProductTypeKeys,
    int? AverageQuantity,
    string? Description,
    AddressRequest? Address,
    List<SocialMediaRequest>? SocialMedia,
    IEnumerable<PhoneRequest>? Phones
);
