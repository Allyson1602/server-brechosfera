using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brechosfera.Application.Handlers;

public sealed class UserDeletedHandler (
    IEmailSender emailSender,
    ILogger<UserDeletedHandler> logger
) : IMessageHandler<UserDeletedMessage> {
    public async Task HandleAsync(UserDeletedMessage message, CancellationToken cancellationToken)
    {
        var name = WebUtility.HtmlEncode(message.Name);

        var html = $"""
            <h2>Olá, {name}!</h2>
            <p>Sua conta no Brechosfera foi excluída.</p>
            <p>Obrigado por ter feito parte da nossa comunidade.</p>
        """;

        await emailSender.SendAsync(message.Email, "Sua conta no Brechosfera foi excluída", html, cancellationToken);

        logger.LogInformation("E-mail de exclusão de conta enviado para o usuário {UserId}", message.UserId);
    }
}
