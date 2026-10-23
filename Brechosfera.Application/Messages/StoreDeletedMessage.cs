namespace Brechosfera.Application.Messages;

public record StoreDeletedMessage(Guid UserId, string UserName, string StoreName, string Email);
