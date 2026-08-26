using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Repositories;

public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<List<ProductType>> ListProductTypeAsync(CancellationToken cancellationToken)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<ProductType>> ListProductTypeByKeyAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(p => keys.Contains(p.Key))
            .ToListAsync(cancellationToken);
    }
}
