namespace Brechosfera.Application.Common;

public sealed class AppUrlSettings
{
    public const string SectionName = "AppUrls";
    public string ConfirmEmail { get; init; } = string.Empty;
}
