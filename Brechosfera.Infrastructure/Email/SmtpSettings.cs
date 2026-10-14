namespace Brechosfera.Infrastructure.Email;

public class SmtpSettings
{
    public const string Section = "Smtp";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public bool EnableSsl { get; set; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = "noreply@demo.local";
    public string FromName { get; set; } = "Onion Demo";
}
