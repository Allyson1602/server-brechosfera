using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class StoreRepository : Repository<Store>, IStoreRepository
{
    public StoreRepository(AppDbContext context) : base(context) { }

    public async Task<Store?> GetStoreByIdAsync (Guid id, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Address)
            .Include(x => x.ProductTypes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
