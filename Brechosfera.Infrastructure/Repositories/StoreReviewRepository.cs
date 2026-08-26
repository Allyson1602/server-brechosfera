using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class StoreReviewRepository : Repository<StoreReview>, IStoreReviewRepository
{
    public StoreReviewRepository(AppDbContext dbContext) : base(dbContext) { }

    async public Task<IEnumerable<StoreReview>> ListStoreReviewByStoreIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        return await DbSet
             .AsNoTracking()
             .Where(s => s.StoreId == storeId)
             .ToListAsync();
    }
}
