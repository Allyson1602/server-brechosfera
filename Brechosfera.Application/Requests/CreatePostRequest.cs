namespace Brechosfera.Application.Requests;

public record CreatePostRequest(
    Guid UserId,
    string? Image,
    string Description
);
