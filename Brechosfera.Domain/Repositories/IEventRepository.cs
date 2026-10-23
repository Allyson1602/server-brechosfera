using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<IReadOnlyList<Event>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<Event>> ListByStoreIdAsync(Guid storeId, CancellationToken cancellationToken);
}
