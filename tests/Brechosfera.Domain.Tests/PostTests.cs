using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Exceptions;

namespace Brechosfera.Domain.Tests;

public class PostTests
{
    private Post CreatePost(
        Guid? userId = null,
        string description = "Descrição do post.",
        string? image = "imagemembase64"
    ) {
        return new(
            userId ?? Guid.Empty,
            description,
            image
        );
    }

    [Fact]
    public void Constructor_WhenUserIdIsEmpty_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => CreatePost());

        Assert.Equal("USER_ID_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenDescriptionIsBlank_ThrowsDomainException(string descriptionPost)
    {
        var exception = Assert.Throws<DomainException>(() => CreatePost(Guid.NewGuid(), description: descriptionPost));

        Assert.Equal("DESCRIPTION_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithoutImage_CreatesPost(string? imagePost)
    {
        var userId = Guid.NewGuid();
        var post = CreatePost(userId, image: imagePost);

        Assert.Equal(userId, post.UserId);
        Assert.Equal("Descrição do post.", post.Description);
        Assert.Null(post.Image);
    }

    [Fact]
    public void UpdatePost_WithoutImage_UpdatesDescription()
    {
        var post = CreatePost(Guid.NewGuid());

        post.UpdatePost("Nova descrição do post.");

        Assert.Equal("Nova descrição do post.", post.Description);
    }

    [Fact]
    public void UpdatePost_WithImage_UpdateDescriptionAndImage()
    {
        var post = CreatePost(Guid.NewGuid(), "Descrição do post.", "imagemembase64");

        post.UpdatePost("Nova descrição do post.", "newimagebase64");

        Assert.Equal("Nova descrição do post.", post.Description);
        Assert.Equal("newimagebase64", post.Image);
    }

    [Fact]
    public void UpdatePost_WhenDescriptionIsBlank_DoesNotChangePost()
    {
        var post = CreatePost(Guid.NewGuid());

        var exception = Assert.Throws<DomainException>(() => post.UpdatePost(""));

        Assert.Equal("DESCRIPTION_EMPTY", exception.Code);
    }

    [Fact]
    public void AddLike_WhenUserIdIsEmpty_ThrowsDomainException()
    {
        var post = CreatePost(Guid.NewGuid());

        var exception = Assert.Throws<DomainException>(() => post.AddLike(Guid.Empty));

        Assert.Equal("LIKE_USER_ID_EMPTY", exception.Code);
    }

    [Fact]
    public void AddLike_WhenUserLikesTwice_KeepsOneLike()
    {
        var userId = Guid.NewGuid();
        var post = CreatePost(Guid.NewGuid(), "Descrição do post.", "imagemembase64");

        post.AddLike(userId);
        post.AddLike(userId);

        Assert.Single(post.Likes);
    }

    [Fact]
    public void AddLike_WhenDifferentUsers_KeepsAllLikes()
    {
        var post = CreatePost(Guid.NewGuid(), "Descrição do post.", "imagemembase64");

        post.AddLike(Guid.NewGuid());
        post.AddLike(Guid.NewGuid());

        Assert.Equal(2, post.Likes.Count);
    }

    [Fact]
    public void RemoveLike_WhenUserHasLiked_RemovesTheLike()
    {
        var userId = Guid.NewGuid();
        var post = CreatePost(Guid.NewGuid(), "Descrição do post.", "imagemembase64");

        post.AddLike(userId);

        Assert.Single(post.Likes);

        post.RemoveLike(userId);

        Assert.Empty(post.Likes);
    }

    [Fact]
    public void RemoveLike_WhenUserHasNotLiked_KeepsOtherLikes()
    {
        var otherUserId = Guid.NewGuid();
        var post = CreatePost(Guid.NewGuid(), "Descrição do post.", "imagemembase64");

        post.AddLike(otherUserId);
        post.RemoveLike(Guid.NewGuid());

        Assert.Equal(otherUserId, Assert.Single(post.Likes).UserId);
    }
}
