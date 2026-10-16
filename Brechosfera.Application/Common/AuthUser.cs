namespace Brechosfera.Application.Common;

public sealed record AuthUser(
    Guid Id,
    string UserName,
    string Email,
    IReadOnlyList<string> Roles
);
