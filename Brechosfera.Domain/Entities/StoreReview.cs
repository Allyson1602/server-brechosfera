using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Guards;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Entities;

public class StoreReview : Entity
{
    public Guid UserId { get; }
    public Guid StoreId { get; }
    public Rating Rating { get; private set; } = null!;
    public string? Comment { get; private set; }

    private StoreReview() { }

    public StoreReview(
        Guid userId,
        Guid storeId,
        Rating rating,
        string? comment
    ) {
        Guard.AgainstEmptyGuid(storeId, "REVIEW_STORE_NOT_FOUND", "A loja é obrigatória.");
        Guard.AgainstEmptyGuid(userId, "REVIEW_USER_NOT_FOUND", "O usuário é obrigatório.");
        Guard.AgainstNullMaxLength(comment, 500, "REVIEW_COMMENT_MAX_LIMIT", "O comentário não pode exceder 500 caracteres.");

        UserId = userId;
        StoreId = storeId;
        Rating = rating;
        Comment = comment;
    }
}
