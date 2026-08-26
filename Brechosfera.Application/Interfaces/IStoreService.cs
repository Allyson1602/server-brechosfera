using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IStoreService
{
    public Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken);
    public Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<StoreResponse?> CreateStoreAsync(StoreRequest request, CancellationToken cancellationToken);
}
