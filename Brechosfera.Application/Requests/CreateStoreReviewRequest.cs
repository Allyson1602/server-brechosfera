namespace Brechosfera.Application.Requests;

public sealed record CreateStoreReviewRequest(
    Guid UserId,
    Guid StoreId,
    int Rating,
    string? Comment
);
