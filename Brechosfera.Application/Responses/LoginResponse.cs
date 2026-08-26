namespace Brechosfera.Application.Responses;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime Expiration
);
