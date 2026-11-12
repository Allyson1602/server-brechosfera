using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class SocialMediaTests
{
    [Fact]
    public void SocialMedia_WhenEmptyLink_ThrowsEmptyLinkException()
    {
        var link = "";
        var platform = ESocialMediaPlatform.Instagram;

        var exception = Assert.Throws<DomainException>(() => new SocialMedia(
            link,
            platform
        ));

        Assert.Equal("SOCIAL_MEDIA_EMPTY_LINK", exception.Code);
    }

    [Fact]
    public void SocialMedia_WhenInvalidPlatform_ThrowsInvalidPlatformException()
    {
        var link = "@brechosfera";
        var platform = (ESocialMediaPlatform)888;

        var exception = Assert.Throws<DomainException>(() => new SocialMedia(
            link,
            platform
        ));

        Assert.Equal("SOCIAL_MEDIA_INVALID_PLATFORM", exception.Code);
    }
}
