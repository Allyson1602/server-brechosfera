namespace Brechosfera.Application.Responses;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    UserSummary User
);
