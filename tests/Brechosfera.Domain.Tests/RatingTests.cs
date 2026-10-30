using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Create_WhenValueIsInRange_CreatesRating(int value)
    {
        var rating = Rating.Create(value);

        Assert.Equal(value, rating.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_WhenValueIsOutOfRange_ThrowsDomainException(int value)
    {
        var exception = Assert.Throws<DomainException>(() => Rating.Create(value));

        Assert.Equal("RATING_OUT_RANGE", exception.Code);
    }
}
