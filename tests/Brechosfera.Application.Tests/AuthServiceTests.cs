using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Repositories;
using Brechosfera.Tests.Common.Factories;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class AuthServiceTests
{
    private const string ConfirmEmailUrl = "https://teste/confirm-email";

    private readonly IIdentityService _identity = Substitute.For<IIdentityService>();
    private readonly IMessagePublisher _publisher = Substitute.For<IMessagePublisher>();
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly IStoreRepository _storeRepository = Substitute.For<IStoreRepository>();
    private readonly ITokenGenerator _tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly Notifier _notifier = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            storeDeletion: new StoreDeletion(_eventRepository),
            storeRepository: _storeRepository,
            identity: _identity,
            tokenGenerator: _tokenGenerator,
            publisher: _publisher,
            notifier: _notifier,
            currentUser: _currentUser,
            unitOfWork: _unitOfWork,
            appUrls: Options.Create(new AppUrlSettings { ConfirmEmail = ConfirmEmailUrl })
        );

        _unitOfWork
            .ExecuteInTransactionAsync(Arg.Any<Func<Task<bool>>>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Func<Task<bool>>>().Invoke());
    }

    private static AuthUser NewAuthUser()
    {
        return new(Guid.NewGuid(), "João Silva", "joaosilva@email.com", []);
    }

    // ---------- ConfirmEmail ----------

    [Fact]
    public async Task ConfirmEmailAsync_WhenNewlyConfirmed_PublishesWelcomeOnce()
    {
        //Arrange
        var user = NewAuthUser();
        _identity.ConfirmEmailAsync(user.Id, "token")
            .Returns((Result.Success(), user));

        //Act
        var result = await _service.ConfirmEmailAsync(user.Id, "token", CancellationToken.None);

        //Assert
        Assert.True(result);
        await _publisher.Received(1).PublishAsync(
            Queues.UserCreated,
            Arg.Is<UserMessage>(m => m.UserId == user.Id && m.Email == user.Email),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task ConfirmEmailAsync_WhenAlreadyConfirmed_DoesNotPublish()
    {
        _identity.ConfirmEmailAsync(Arg.Any<Guid>(), Arg.Any<string>())
            .Returns((Result.Success(), (AuthUser?)null));

        var result = await _service.ConfirmEmailAsync(Guid.NewGuid(), "token", CancellationToken.None);

        Assert.True(result);
        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync<UserMessage>(default!, default!, default);
    }

    // ---------- ResendConfirmation ----------

    [Fact]
    public async Task ResendConfirmationAsync_WhenEmailDoesNotExist_DoesNotPublishAndDoesNotNotify()
    {
        _identity.FindUnconfirmedByEmailAsync("notexist@email.com")
            .Returns((AuthUser?)null);

        await _service.ResendConfirmationAsync(
            new ResendConfirmationRequest("naoexiste@email.com"), CancellationToken.None
        );

        Assert.False(_notifier.HasNotifications());

        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync<UserMessage>(default!, default!, default);
    }

    [Fact]
    public async Task ResendConfirmationAsync_WhenUserIsUnconfirmed_PublishesConfirmationEmail()
    {
        var user = NewAuthUser();

        _identity.FindUnconfirmedByEmailAsync(user.Email).Returns(user);
        _identity.GenerateEmailConfirmationTokenAsync(user.Id).Returns("abc123");

        await _service.ResendConfirmationAsync(
            new ResendConfirmationRequest(user.Email),
            CancellationToken.None
        );

        await _publisher.Received(1).PublishAsync(
            Queues.EmailConfirmationRequested,
            Arg.Is<EmailConfirmationMessage>(m =>
                m.UserId == user.Id &&
                m.ConfirmationLink == $"{ConfirmEmailUrl}?userId={user.Id}&token=abc123"),
            Arg.Any<CancellationToken>()
        );
    }

    // ---------- Login ----------

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreInvalid_ReturnsNullAndNotifies()
    {
        _identity.CheckCredentialsAsync("joao", "errada")
            .Returns((Result.Failure(["Login ou senha inválidos."]), null));

        var result = await _service.LoginAsync(new LoginRequest("joao", "errada"));

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "Login");

        _tokenGenerator.DidNotReceiveWithAnyArgs().Generate(default!);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsToken()
    {
        var user = NewAuthUser();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        _identity.CheckCredentialsAsync("joao", "certa")
            .Returns((Result.Success(), user));
        _tokenGenerator.Generate(user)
            .Returns(("jwt-token", expiresAt));

        var result = await _service.LoginAsync(new LoginRequest("joao", "certa"));

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal(user.Id, result.User.Id);
        Assert.False(_notifier.HasNotifications());
    }

    // ---------- DeleteAccount ----------

    [Fact]
    public async Task DeleteAccountAsync_WhenIdentitySucceeds_PublishesUserDeleted()
    {
        var user = NewAuthUser();
        var store = StoreFactory.Create(ownerUserId: user.Id);

        _currentUser.UserId.Returns(user.Id);
        _storeRepository.ListByOwnerAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns([store]);
        _identity.DeleteAccountAsync(user.Id)
            .Returns((Result.Success(), user));

        await _service.DeleteAccountAsync(CancellationToken.None);

        Assert.NotNull(store.DeletedAt);
        Assert.False(_notifier.HasNotifications());

        await _publisher.Received(1).PublishAsync(
            Queues.UserDeleted,
            Arg.Is<UserDeletedMessage>(m => m.UserId == user.Id),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task DeleteAccountAsync_WhenIdentityFails_NotifiesAndDoesNotPublish()
    {
        var userId = Guid.NewGuid();

        _currentUser.UserId.Returns(userId);
        _storeRepository.ListByOwnerAsync(userId, Arg.Any<CancellationToken>())
            .Returns([]);
        _identity.DeleteAccountAsync(userId)
            .Returns((Result.Failure(["Falha ao excluir."]), null));

        await _service.DeleteAccountAsync(CancellationToken.None);

        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "DeleteAccount");

        await _publisher.DidNotReceiveWithAnyArgs()
            .PublishAsync<UserDeletedMessage>(default!, default!, default);
    }
}
