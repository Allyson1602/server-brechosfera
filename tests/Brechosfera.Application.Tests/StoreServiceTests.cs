using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Services;
using Brechosfera.Application.Tests.Factories;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class StoreServiceTests
{
    private readonly IStoreRepository _storeRepository = Substitute.For<IStoreRepository>();
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IIdentityService _identity = Substitute.For<IIdentityService>();
    private readonly IMessagePublisher _publisher = Substitute.For<IMessagePublisher>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Notifier _notifier = new();
    private readonly StoreService _service;

    public StoreServiceTests()
    {
        _service = new StoreService(
            publisher: _publisher,
            unitOfWork: _unitOfWork,
            notifier: _notifier,
            identity: _identity,
            repository: _storeRepository,
            productTypeRepository: Substitute.For<IProductTypeRepository>(),
            currentUser: _currentUser,
            storeDeletion: new StoreDeletion(_eventRepository)
        );
    }

    [Fact]
    public async Task CreateStoreAsync_WhenNameAlreadyExists_DoesNotCreateStore()
    {
        _storeRepository.ExistsByNameAsync("Brechó Aurora", Arg.Any<Guid?>(), Arg.Any<CancellationToken>()).Returns(true);

        var request = new CreateStoreRequest(
            Name: " Brechó Aurora ",
            AveragePrice: 50m,
            LocalType: Domain.Enums.ELocalType.Physical,
            ProductTypeKeys: ["camisetas"],
            AverageQuantity: null,
            Description: null,
            Address: null,
            SocialMedia: null,
            Phones: null
        );

        var result = await _service.CreateStoreAsync(request, CancellationToken.None);

        Assert.Null(result);
        Assert.Contains(_notifier.GetNotifications(), n => n.Code == "Name");
        _storeRepository.DidNotReceive().Add(Arg.Any<Store>());
    }

    [Fact]
    public async Task DeleteStoreAsync_WhenUserIsOwner_DeletesStoreAndNotifiesOwner()
    {
        var ownerId = Guid.NewGuid();
        var store = StoreFactory.Create(ownerUserId: ownerId);
        var owner = new AuthUser(ownerId, "ana", "ana@email.com", []);

        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _currentUser.UserId.Returns(ownerId);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(true);
        _identity.GetByIdAsync(ownerId).Returns(owner);
        _eventRepository.ListByStoreIdsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<Event>());

        var result = await _service.DeleteStoreAsync(store.Id, CancellationToken.None);

        Assert.True(result);
        Assert.NotNull(store.DeletedAt);
        Assert.False(_notifier.HasNotifications());

        await _publisher.Received(1).PublishAsync(
            Queues.StoreDeleted,
            Arg.Is<StoreDeletedMessage>(m => m.Email == "ana@email.com" && m.StoreName == store.Name),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task DeleteStoreAsync_WhenUserIsNotOwner_DoesNotDeleteNorSendEmail()
    {
        var store = StoreFactory.Create(ownerUserId: Guid.NewGuid());
        _storeRepository.GetByIdAsync(store.Id, Arg.Any<CancellationToken>()).Returns(store);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var result = await _service.DeleteStoreAsync(store.Id, CancellationToken.None);

        Assert.False(result);
        Assert.Null(store.DeletedAt);
        Assert.True(_notifier.HasNotifications());

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync<StoreDeletedMessage>(default!, default!, default!);
    }
}
