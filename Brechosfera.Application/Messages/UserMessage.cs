namespace Brechosfera.Application.Messages;

public record UserMessage(Guid UserId, string Name, string Email, DateTimeOffset CreatedAt, string ConfirmationLink);