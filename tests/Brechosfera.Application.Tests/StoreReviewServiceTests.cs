using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Tests.Common.Factories;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class StoreReviewServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IStoreReviewRepository _repository = Substitute.For<IStoreReviewRepository>();
    private readonly IStoreRepository _storeRepository = Substitute.For<IStoreRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Notifier _notifier = new();
    private readonly StoreReviewService _service;

    public StoreReviewServiceTests()
    {
        _service = new StoreReviewService(
            unitOfWork: _unitOfWork,
            notifier: _notifier,
            repository: _repository,
            storeRepository: _storeRepository,
            currentUser: _currentUser);

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    private static CreateStoreReviewRequest NewCreateRequest(Guid storeId) => new(
        storeId,
        4,
        "Ótimas peças!");

    private Store ArrangeStore(Guid? ownerId = null)
    {
        var store = StoreFactory.Create(ownerUserId: ownerId);
        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);

        return store;
    }

    [Fact]
    public async Task AddStoreReviewAsync_WhenValid_AddsReview()
    {
        var userId = Guid.NewGuid();
        var store = ArrangeStore();

        _currentUser.UserId.Returns(userId);

        var result = await _service.AddStoreReviewAsync(NewCreateRequest(store.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(_notifier.HasNotifications());

        _repository.Received(1).Add(Arg.Is<StoreReview>(r =>
            r.UserId == userId && r.StoreId == store.Id && r.Rating.Value == 4));

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddStoreReviewAsync_WhenStoreDoesNotExist_NotifiesStoreNotFound()
    {
        var result = await _service.AddStoreReviewAsync(NewCreateRequest(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "STORE_NOT_FOUND");

        _repository.DidNotReceiveWithAnyArgs().Add(default!);

        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact]
    public async Task AddStoreReviewAsync_WhenUserOwnsStore_NotifiesCannotReviewOwnStore()
    {
        var ownerId = Guid.NewGuid();
        var store = ArrangeStore(ownerId);

        _currentUser.UserId.Returns(ownerId);

        var result = await _service.AddStoreReviewAsync(NewCreateRequest(store.Id), CancellationToken.None);

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "STORE_REVIEW_OWN_STORE");
        _repository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact]
    public async Task AddStoreReviewAsync_WhenUserAlreadyReviewed_NotifiesAlreadyReviewed()
    {
        var store = ArrangeStore();
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        _repository.ExistsByUserAndStoreAsync(userId, store.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _service.AddStoreReviewAsync(NewCreateRequest(store.Id), CancellationToken.None);

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "STORE_REVIEW_ALREADY_EXISTS");
        _repository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }
}
