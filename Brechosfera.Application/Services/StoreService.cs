using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Messages;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Brechosfera.Application.Services;

public class StoreService : BaseService, IStoreService
{
    private readonly IDistributedCache _cache;
    private readonly IStoreRepository _repository;
    private readonly IProductTypeRepository _productTypeRepository;
    private readonly IIdentityService _identity;
    private readonly ICurrentUser _currentUser;
    private readonly StoreDeletion _storeDeletion;
    private readonly IMessagePublisher _publisher;

    public StoreService(
        StoreDeletion storeDeletion,
        IMessagePublisher publisher,
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IIdentityService identity,
        IStoreRepository repository,
        IProductTypeRepository productTypeRepository,
        ICurrentUser currentUser,
        IDistributedCache cache
    ) : base(unitOfWork, notifier) {
        _cache = cache;
        _storeDeletion = storeDeletion;
        _identity = identity;
        _publisher = publisher;
        _repository = repository;
        _productTypeRepository = productTypeRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> DeleteStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var store = await _repository.GetByIdAsync(storeId, cancellationToken);

        if (store is null)
        {
            Notify(
                "StoreId",
                "A loja informado não foi encontrada."
            );

            return false;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify(
                "StoreAccessDenied",
                "Você não tem permissão para excluir esta loja."
            );
            return false;
        }

        await _storeDeletion.MarkAsDeletedAsync([store], cancellationToken);

        var success = await CommitAsync(cancellationToken);

        if (!success)
            return false;

        var owner = await _identity.GetByIdAsync(store.OwnerUserId);

        if (owner is not null)
        {
            await _publisher.PublishAsync(
                Queues.StoreDeleted,
                new StoreDeletedMessage(owner.Id, owner.UserName, store.Name, owner.Email),
                cancellationToken
            );
        }

        await _cache.RemoveAsync($"store:{storeId}", cancellationToken);

        return true;
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

        store.RemoveSocialMedia(request.Link);
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);
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
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);

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

        store.RemovePhone(request.PhoneNumber);
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);
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
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);
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
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);
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

        var newProductTypeIds = request.ProductTypeIds.Distinct().ToList();
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
        var success = await CommitAsync(cancellationToken);

        if (success)
            await _cache.RemoveAsync($"store:{id}", cancellationToken);
    }

    public async Task<StoreResponse?> UpdateStoreAsync(Guid id, UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        Store? store = await _repository.GetByIdAsync(id, cancellationToken);

        if (store is null)
        {
            Notify("STORE_NOT_FOUND", "A loja não foi encontrada.");
            return null;
        }

        if (!store.CheckIsOwnedBy(_currentUser.UserId))
        {
            Notify("STORE_ACCESS_DENIED", "Você não tem permissão para alterar esta loja.");
            return null;
        }

        var name = request.Name.Trim();
        var isExistsByName = await _repository.ExistsByNameAsync(name, store.Id, cancellationToken);

        if (isExistsByName)
        {
            Notify("Name", "Já existe uma loja com esse nome.");
            return null;
        }

        store.UpdateDetails(
            name,
            request.AveragePrice,
            request.LocalType,
            request.AverageQuantity,
            request.Description
        );

        var success = await CommitAsync(cancellationToken);

        if (!success)
            return null;

        await _cache.RemoveAsync($"store:{id}", cancellationToken);
        return store.ToResponse();
    }

    public async Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Store> stores = await _repository.GetAllAsync(cancellationToken);

        return stores.ToResponseList();
    }

    public async Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"store:{id}";
        var cachedStore = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedStore))
            return JsonSerializer.Deserialize<StoreResponse>(cachedStore);

        Store? store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store is null)
            return null;

        var response = store.ToResponse();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return response;
    }

    public async Task<StoreResponse?> CreateStoreAsync(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var isExistsByName = await _repository.ExistsByNameAsync(name, cancellationToken: cancellationToken);

        if (isExistsByName)
        {
            Notify("Name", "Já existe uma loja com esse nome.");
            return null;
        }

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
            name,
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

        if (!success)
            return null;

        return newStore.ToResponse();
    }
}
