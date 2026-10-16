namespace Brechosfera.Application.Requests;

public sealed record CreateStoreReviewRequest(
    Guid StoreId,
    int Rating,
    string? Comment
);
