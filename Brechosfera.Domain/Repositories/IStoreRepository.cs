using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;

public interface IStoreRepository : IRepository<Store> {
    public Task<Store?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken);
}
