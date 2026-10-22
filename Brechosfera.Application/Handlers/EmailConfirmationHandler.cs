using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brechosfera.Application.Handlers;

public sealed class EmailConfirmationHandler(
    IEmailSender emailSender,
    ILogger<EmailConfirmationHandler> logger
) : IMessageHandler<EmailConfirmationMessage>
{
    public async Task HandleAsync (EmailConfirmationMessage message, CancellationToken cancellationToken)
    {
        var nome = WebUtility.HtmlEncode(message.Name);
        var link = WebUtility.HtmlEncode(message.ConfirmationLink);

        var html = $"""
            <h2>Olá, {nome}!</h2>
            <p>Para ativar sua conta no Brechosfera, confirme seu e-mail:</p>
            <p><a href="{link}">Confirmar e-mail</a></p>
            <p>Se você não criou uma conta, ignore este e-mail.</p>
        """;

        await emailSender.SendAsync(message.Email, "Confirme seu e-mail", html, cancellationToken);

        logger.LogInformation("E-mail de confirmação enviado para o usuário {UserId}", message.UserId);
    }
}
