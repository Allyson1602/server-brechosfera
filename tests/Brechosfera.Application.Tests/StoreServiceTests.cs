using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Services;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Repositories;
using NSubstitute;
using Brechosfera.Application.Tests.Factories;

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
