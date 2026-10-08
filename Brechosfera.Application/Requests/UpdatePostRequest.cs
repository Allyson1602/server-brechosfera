namespace Brechosfera.Application.Requests;

public sealed record UpdatePostRequest(
    string Image,
    string Description
);
