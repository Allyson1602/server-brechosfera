using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Mapper;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Repositories;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Services;

public class StoreService : BaseService, IStoreService
{
    private readonly IStoreRepository _repository;
    private readonly IProductTypeRepository _productTypeRepository;

    public StoreService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IStoreRepository repository,
        IProductTypeRepository productTypeRepository
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _productTypeRepository = productTypeRepository;
    }

    public async Task<IReadOnlyList<StoreResponse>> ListStoreAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Store> stores = await _repository.GetAllAsync(cancellationToken);

        return stores.ToResponseList();
    }

    public async Task<StoreResponse?> GetStoreByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Store? store = await _repository.GetStoreByIdAsync(id, cancellationToken);

        if (store == null)
            return null;

        return store.ToResponse();
    }

    public async Task<StoreResponse?> CreateStoreAsync(StoreRequest request, CancellationToken cancellationToken)
    {
        List<ProductType> productTypes = await _productTypeRepository.ListProductTypeByKeyAsync(request.ProductTypeKeys, cancellationToken);
        Address? newAddress = null;

        var uniqueKeys = request.ProductTypeKeys.Distinct().Count();

        if (productTypes.Count != uniqueKeys)
        {
            Notify("ProductTypeKeys", "Um ou mais tipos de produto inválido.");
            return null;
        }

        Store newStore = new Store(
            request.Name,
            request.ResponsibleName,
            request.AveragePrice,
            (ELocalType)request.LocalType,
            productTypes,
            request.Description,
            request.AverageQuantity,
            request.socialMedia,
            request.phones
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