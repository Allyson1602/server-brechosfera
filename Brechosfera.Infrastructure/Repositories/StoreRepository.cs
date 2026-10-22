using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class StoreRepository : Repository<Store>, IStoreRepository
{
    public StoreRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string name, Guid? ignoreStoreId = null, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name && x.Id != ignoreStoreId, cancellationToken);
    }

    public async Task<Store?> GetStoreByIdAsync (Guid id, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(x => x.Address)
            .Include(x => x.ProductTypes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
