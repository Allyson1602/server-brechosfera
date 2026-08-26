using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Responses;

public sealed record StoreReviewResponse(
    Guid Id,
    Guid userId,
    Guid storeId,
    int rating,
    string? comment
);
