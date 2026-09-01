using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IStoreReviewService
{
    public Task<List<StoreReviewResponse>> ListStoreReviewByStoreIdAsync(Guid storeId, CancellationToken cancellationToken);
    public Task<StoreReviewResponse?> GetStoreReviewByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<StoreReviewResponse?> AddStoreReviewAsync(CreateStoreReviewRequest request, CancellationToken cancellationToken);
}
