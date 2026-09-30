using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;

namespace Brechosfera.Domain.Repositories;

public interface IProductTypeRepository : IRepository<ProductType>
{
    public Task<IReadOnlyList<ProductType>> ListProductTypesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    public Task<List<ProductType>> ListProductTypeAsync(CancellationToken cancellationToken);
    public Task<List<ProductType>> ListProductTypeByKeyAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
}
