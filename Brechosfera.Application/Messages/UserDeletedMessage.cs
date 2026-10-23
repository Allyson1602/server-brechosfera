namespace Brechosfera.Application.Messages;

public record UserDeletedMessage(Guid UserId, string Name, string Email);
