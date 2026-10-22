namespace Brechosfera.Application.Messages;

public sealed record EmailConfirmationMessage(Guid UserId, string Name, string Email, string ConfirmationLink);
