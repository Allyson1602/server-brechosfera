using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IStoreService
{
    public Task<StoreResponse?> UpdateStoreAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken);
    public Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken);
    public Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<StoreResponse?> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken);
}
