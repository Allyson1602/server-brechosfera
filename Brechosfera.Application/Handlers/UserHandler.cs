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

        var html = $"""
            <h2>Olá, {nome}!</h2>
            <p>Seu cadastro foi realizado com sucesso em {message.CreatedAt:dd/MM/yyyy HH:mm} (UTC).</p>
        """;

        await emailSender.SendAsync(message.Email, "Bem-vindo(a)!", html, cancellationToken);

        logger.LogInformation("E-mail de boas-vindas enviado para {Email}", message.Email);
    }
}
