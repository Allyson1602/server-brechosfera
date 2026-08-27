using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Responses;

public sealed record StoreResponse(
    Guid Id,
    Guid OwnerUserId,
    string Name,
    decimal AveragePrice,
    ELocalType LocalType,
    List<ProductTypeResponse> ProductType,
    int? AverageQuantity,
    string? Description,
    AddressResponse? Address,
    IReadOnlyCollection<StoreReview> reviews,
    IReadOnlyCollection<Phone> phones,
    IReadOnlyCollection<SocialMedia> socialMedia
);
