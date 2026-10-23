using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brechosfera.Application.Handlers;

public sealed class StoreDeletedHandler(
    IEmailSender emailSender,
    ILogger<StoreDeletedHandler> logger
) : IMessageHandler<StoreDeletedMessage> {
    public async Task HandleAsync(StoreDeletedMessage message, CancellationToken cancellationToken)
    {
        var userName = WebUtility.HtmlEncode(message.UserName);
        var storeName = WebUtility.HtmlEncode(message.StoreName);

        var html = $"""
            <h2>Olá, {userName}!</h2>
            <p>Sua loja <strong>{storeName}</strong> no Brechosfera foi excluída.</p>
            <p>Se não foi você quem fez isso, altere sua senha e entre em contato conosco.</p>
        """;

        await emailSender.SendAsync(message.Email, "Sua loja no Brechosfera foi excluída", html, cancellationToken);

        logger.LogInformation("E-mail de exclusão de loja enviado para o usuário {UserId}", message.UserId);
    }
}
