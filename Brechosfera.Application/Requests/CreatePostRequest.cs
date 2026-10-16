namespace Brechosfera.Application.Requests;

public record CreatePostRequest(
    string? Image,
    string Description
);
