namespace Brechosfera.Application.Requests;

public sealed record UpdatePostRequest(
    string Description,
    string? Image
);
