using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Responses;

public record PostResponse(
    Guid Id,
    Guid UserId,
    string? Image,
    string Description,
    IReadOnlyCollection<PostLike> Likes
);
