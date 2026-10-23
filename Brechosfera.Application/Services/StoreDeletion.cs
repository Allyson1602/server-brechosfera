using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;

namespace Brechosfera.Application.Services;

public sealed class StoreDeletion (IEventRepository eventRepository)
{
    public async Task MarkAsDeletedAsync(IReadOnlyCollection<Store> stores, CancellationToken cancellationToken)
    {
        if (stores.Count == 0)
            return;

        foreach (var store in stores)
            store.MarkAsDeleted();

        var storeIds = stores.Select(s => s.Id).ToList();
        var events = await eventRepository.ListByStoreIdsAsync(storeIds, cancellationToken);

        foreach (var storeEvent in events)
            storeEvent.MarkAsDeleted();
    }
}
