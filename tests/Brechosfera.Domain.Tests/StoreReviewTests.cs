using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class StoreReviewTests
{
    [Fact]
    public void Constructor_WhenCommentExceeds500Characters_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new StoreReview(
            userId: Guid.NewGuid(),
            storeId: Guid.NewGuid(),
            rating: Rating.Create(5),
            comment: new string('a', 501)
        ));

        Assert.Equal("REVIEW_COMMENT_MAX_LIMIT", exception.Code);
    }

    [Fact]
    public void Constructor_WhenCommentIsNull_CreatesReview()
    {
        var review = new StoreReview(
            userId: Guid.NewGuid(),
            storeId: Guid.NewGuid(),
            rating: Rating.Create(5),
            comment: null
        );

        Assert.Null(review.Comment);
    }
}
