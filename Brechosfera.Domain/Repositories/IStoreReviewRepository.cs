using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;

public interface IStoreReviewRepository : IRepository<StoreReview>
{
    public Task<IEnumerable<StoreReview>> ListStoreReviewByStoreIdAsync(Guid storeId, CancellationToken cancellationToken);
}
