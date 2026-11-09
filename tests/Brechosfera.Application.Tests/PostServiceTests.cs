using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class PostServiceTests
{
    private readonly IRepository<Post> _repository = Substitute.For<IRepository<Post>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Notifier _notifier = new();
    private readonly PostService _service;

    public PostServiceTests()
    {
        _service = new PostService(
            unitOfWork: _unitOfWork,
            notifier: _notifier,
            repository: _repository,
            currentUser: _currentUser
        );

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    [Fact]
    public async Task DeletePostAsync_WhenUserIsAuthor_DeletesPost()
    {
        var userId = Guid.NewGuid();
        var post = new Post(userId: userId, description: "Descrição");

        _repository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);
        _currentUser.UserId.Returns(userId);

        var result = await _service.DeletePostAsync(post.Id, CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(post.DeletedAt);
        Assert.False(_notifier.HasNotifications());

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeletePostAsync_WhenUserIsNotAuthor_DoesNotDeletePost()
    {
        var post = new Post(userId: Guid.NewGuid(), description: "Descrição original.");

        _repository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.DeletePostAsync(post.Id, CancellationToken.None);

        Assert.False(result);
        Assert.Null(post.DeletedAt);
    }

    [Fact]
    public async Task UpdatePostAsync_WhenUserIsAuthor_UpdatesPost()
    {
        var userId = Guid.NewGuid();
        var post = new Post(userId: userId, description: "Descrição original.");

        _repository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);
        _currentUser.UserId.Returns(userId);

        var result = await _service.UpdatePostAsync(
            post.Id,
            new UpdatePostRequest(Description: "Editado pelo autor.", Image: null),
            CancellationToken.None
        );

        Assert.Equal("Editado pelo autor.", result?.Description);
        Assert.Null(result?.Image);
        Assert.False(_notifier.HasNotifications());

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdatePostAsync_WhenUserIsNotAuthor_DoesNotUpdatePost()
    {
        var post = new Post(userId: Guid.NewGuid(), description: "Descrição original.");

        _repository.GetByIdAsync(post.Id, Arg.Any<CancellationToken>()).Returns(post);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.UpdatePostAsync(
            post.Id,
            new UpdatePostRequest(Description: "Editado por outra pessoa.", Image: null),
            CancellationToken.None
        );

        Assert.Null(result);
        Assert.Equal("Descrição original.", post.Description);
        Assert.True(_notifier.HasNotifications());

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
