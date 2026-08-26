namespace Brechosfera.Application.Requests;

public sealed record StoreReviewRequest(
    Guid UserId,
    Guid StoreId,
    int Rating,
    string? Comment
);
