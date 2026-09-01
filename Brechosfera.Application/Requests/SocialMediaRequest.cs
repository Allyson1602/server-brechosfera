using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Requests;

public sealed record SocialMediaRequest(string Link, ESocialMediaPlatform Platform);
