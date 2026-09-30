using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IStoreService
{
    public Task RemoveSocialMediaAsync(Guid id, RemoveSocialMediaRequest request, CancellationToken cancellationToken);
    public Task AddSocialMediaAsync(Guid id, SocialMediaRequest request, CancellationToken cancellationToken);
    public Task RemovePhoneAsync(Guid id, RemovePhoneRequest request, CancellationToken cancellationToken);
    public Task AddPhoneAsync(Guid id, PhoneRequest request, CancellationToken cancellationToken);
    public Task UpdateAddressAsync(Guid id, AddressRequest request, CancellationToken cancellationToken);
    public Task UpdateProductTypesAsync(Guid id, ReplaceStoreProductTypesRequest request, CancellationToken cancellationToken);
    public Task<StoreResponse?> UpdateStoreAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken);
    public Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken);
    public Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<StoreResponse?> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken);
}
