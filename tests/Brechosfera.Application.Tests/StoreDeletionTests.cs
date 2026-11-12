using Brechosfera.Application.Services;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Tests.Common.Factories;
using NSubstitute;

namespace Brechosfera.Application.Tests;

public class StoreDeletionTests
{
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly StoreDeletion _storeDeletion;

    public StoreDeletionTests()
    {
        _storeDeletion = new StoreDeletion(_eventRepository);
    }

    [Fact]
    public async Task MarkAsDeletedAsync_WithStores_MarksStoresAndTheirEventsAsDeleted()
    {
        var stores = new List<Store>()
        {
            StoreFactory.Create(),
            StoreFactory.Create()
        };

        var events = new List<Event>()
        {
            EventFactory.Create(responsibleStoreId: stores[0].Id),
            EventFactory.Create(responsibleStoreId: stores[1].Id)
        };

        _eventRepository.ListByStoreIdsAsync(
            Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(stores[0].Id) && ids.Contains(stores[1].Id)),
            Arg.Any<CancellationToken>()
        ).Returns(events);

        await _storeDeletion.MarkAsDeletedAsync(stores, CancellationToken.None);

        Assert.All(stores, s => Assert.NotNull(s.DeletedAt));
        Assert.All(events, e => Assert.NotNull(e.DeletedAt));
    }

    [Fact]
    public async Task MarkAsDeletedAsync_WithEmptyList_DoesNotQueryEvents()
    {
        await _storeDeletion.MarkAsDeletedAsync([], CancellationToken.None);

        await _eventRepository.DidNotReceiveWithAnyArgs().ListByStoreIdsAsync(default!, default);
    }
}
