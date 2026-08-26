namespace Brechosfera.Application.Requests;

public record PostRequest(
    Guid UserId,
    string? Image,
    string Description
);
