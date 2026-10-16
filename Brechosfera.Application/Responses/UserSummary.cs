namespace Brechosfera.Application.Responses;

public sealed record UserSummary(Guid Id, string UserName, string Email, IReadOnlyList<string> Roles);