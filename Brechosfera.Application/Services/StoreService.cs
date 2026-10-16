using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Services;

public class StoreService : BaseService, IStoreService
{
    private readonly IStoreRepository _repository;
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly ICurrentUser _currentUser;

    public StoreService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IStoreRepository repository,
        IProductTypeRepository productTypeRepository,
        ICurrentUser currentUser
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _productTypeRepository = productTypeRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> DeleteStoreAsync(Guid id, CancellationToken cancellationToken)
    {
        var store = await _repository.GetByIdAsync(id);

        if (store is null)
        {
            Notify(
                "StoreId",
                "A loja informado não foi encontrado."
            );

            return false;
        }

        if (store.DeletedAt.HasValue)
        {
            Notify(
                "DeleteStore",
                "A loja já foi deletada."
            );

            return false;
        }

        store.MarkAsDeleted();

        var success = await CommitAsync(cancellationToken);

        return success;
    }

    public async Task RemoveSocialMediaAsync(Guid id, RemoveSocialMediaRequest request, CancellationToken cancellationToken)
    {
        var store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        store.RemoveSocialMedia(request.link);
        await CommitAsync(cancellationToken);
    }

    public async Task AddSocialMediaAsync(Guid id, SocialMediaRequest request, CancellationToken cancellationToken)
    {
        var store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        var newSocialMedia = new SocialMedia(request.Link, request.Platform);

        store.AddSocialMedia(newSocialMedia);
        await CommitAsync(cancellationToken);
    }

    public async Task RemovePhoneAsync(Guid id, RemovePhoneRequest request, CancellationToken cancellationToken)
    {
        var store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        store.RemovePhone(request.phoneNumber);
        await CommitAsync(cancellationToken);
    }

    public async Task AddPhoneAsync(Guid id, PhoneRequest request, CancellationToken cancellationToken)
    {
        var store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        var newPhone = new Phone(
            request.Number,
            request.IsPrimary
        );

        store.AddPhone(newPhone);
        await CommitAsync(cancellationToken);
    }

    public async Task UpdateAddressAsync(Guid id, AddressRequest request, CancellationToken cancellationToken)
    {
        var store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        var newAddress = new Address(
            request.Cep,
            request.Street,
            request.Neighborhood,
            request.City,
            request.StateCode,
            request.State,
            request.Complement
        );

        store.SetAddress(newAddress);
        await CommitAsync(cancellationToken);
    }

    public async Task UpdateProductTypesAsync(Guid id, ReplaceStoreProductTypesRequest request, CancellationToken cancellationToken)
    {
        Store? store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return;
        }

        var newProductTypeIds = request.productTypeIds.Distinct().ToList();
        var productTypes = await _productTypeRepository.ListProductTypesByIdsAsync(newProductTypeIds, cancellationToken);

        if (productTypes.Count < 1)
        {
            Notify("PRODUCT_TYPE_MINIMUN", "Deve possuir pelo menos 1 tipo de produto.");
            return;
        }

        if (productTypes.Count != newProductTypeIds.Count)
        {
            Notify("PRODUCT_TYPE_NOT_FOUND", "Um ou mais tipos de produto não foram encontrados.");
            return;
        }

        store.ReplaceProductTypes(productTypes);
        await CommitAsync(cancellationToken);
    }

    public async Task<StoreResponse?> UpdateStoreAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        Store? store = await _repository.GetByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return null;
        }

        store.UpdateDetails(
            request.Name,
            request.AveragePrice,
            request.LocalType,
            request.AverageQuantity,
            request.Description
        );

        var success = await CommitAsync(cancellationToken);

        return success ? store.ToResponse() : null;
    }

    public async Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Store> stores = await _repository.GetAllAsync(cancellationToken);

        return stores.ToResponseList();
    }

    public async Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Store? store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
            return null;

        return store.ToResponse();
    }

    public async Task<StoreResponse?> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        List<ProductType> productTypes = await _productTypeRepository.ListProductTypeByKeyAsync(request.ProductTypeKeys, cancellationToken);
        Address? newAddress = null;

        var uniqueKeys = request.ProductTypeKeys.Distinct().Count();

        if (productTypes.Count != uniqueKeys)
        {
            Notify("ProductTypeKeys", "Um ou mais tipos de produto inválido.");
            return null;
        }

        var newSocialMedia = request.SocialMedia?.Select(x => new SocialMedia(x.Link, x.Platform)).ToList();
        var newPhones = request.Phones?.Select(x => new Phone(x.Number, x.IsPrimary)).ToList();

        Store newStore = new Store(
            _currentUser.UserId,
            request.Name,
            request.AveragePrice,
            request.LocalType,
            productTypes,
            request.Description,
            request.AverageQuantity,
            newSocialMedia,
            newPhones
        );

        if (request.Address is not null)
        {
            newAddress = new Address(
                request.Address.Cep,
                request.Address.Street,
                request.Address.Neighborhood,
                request.Address.City,
                request.Address.StateCode,
                request.Address.State,
                request.Address.Complement
            );

            newStore.SetAddress(newAddress);
        }

        _repository.Add(newStore);

        var success = await CommitAsync(cancellationToken);

        return success ? newStore.ToResponse() : null;
    }
}