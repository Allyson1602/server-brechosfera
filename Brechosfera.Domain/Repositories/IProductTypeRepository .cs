using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Domain.Repositories;

public interface IProductTypeRepository : IRepository<ProductType>
{
    public Task<List<ProductType>> ListProductTypeAsync(CancellationToken cancellationToken);
    public Task<List<ProductType>> ListProductTypeByKeyAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
}
