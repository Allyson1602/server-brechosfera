using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;

public interface IStoreRepository : IRepository<Store> {
    public Task<IReadOnlyList<Store>> ListByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
    public Task<bool> ExistsByNameAsync(string name, Guid? ignoreStoreId = null, CancellationToken cancellationToken = default);
    public Task<Store?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken);
}
