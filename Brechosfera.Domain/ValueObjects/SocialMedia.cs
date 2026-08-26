using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Guards;

namespace Brechosfera.Domain.ValueObjects;

public sealed record SocialMedia
{
    public string Link { get; init; } = string.Empty;
    public ESocialMediaPlatform Platform { get; init; }

    private SocialMedia() { }

    public SocialMedia(string link, ESocialMediaPlatform platform)
    {
        Guard.AgainstInvalidEnum(platform, "SOCIAL_MEDIA_INVALID_PLATFORM", "A plataforma é inválida.");
        Guard.AgainstNullOrWhiteSpace(link, "SOCIAL_MEDIA_EMPTY_LINK", "O link da rede social não pode ser vazio.");

        Link = link;
        Platform = platform;
    }
}
