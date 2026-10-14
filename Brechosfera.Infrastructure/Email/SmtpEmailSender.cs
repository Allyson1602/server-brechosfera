using Brechosfera.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Brechosfera.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly IOptions<SmtpSettings> _options;

    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _options = options;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    ) {
        var settings = _options.Value;

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(settings.User))
            client.Credentials = new NetworkCredential(settings.User, settings.Password);

        using var mail = new MailMessage
        {
            From = new MailAddress(settings.From, settings.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await client.SendMailAsync(mail, cancellationToken);
    }
}
