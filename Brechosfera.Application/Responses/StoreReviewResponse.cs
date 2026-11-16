namespace Brechosfera.Application.Responses;

public sealed record StoreReviewResponse(
    Guid Id,
    Guid UserId,
    Guid StoreId,
    int Rating,
    string? Comment
);
