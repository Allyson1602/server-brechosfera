using System.Net;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Microsoft.Extensions.Logging;

namespace Brechosfera.Application.Handlers;

public sealed class UserHandler(
    IEmailSender emailSender,
    ILogger<UserHandler> logger
) : IMessageHandler<UserMessage> {
    public async Task HandleAsync(UserMessage message, CancellationToken cancellationToken)
    {
        var nome = WebUtility.HtmlEncode(message.Name);
        var link = WebUtility.HtmlEncode(message.ConfirmationLink);

        var html = $"""
            <h2>Olá, {nome}!</h2>
            <p>Seu cadastro foi realizado com sucesso. Confirme seu e-mail para entrar:</p>
            <p><a href="{link}">Confirmar e-mail</a></p>
        """;

        await emailSender.SendAsync(message.Email, "Bem-vindo(a)!", html, cancellationToken);

        logger.LogInformation("E-mail de boas-vindas enviado para {Email}", message.Email);
    }
}
