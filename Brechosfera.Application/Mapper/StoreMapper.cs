using Brechosfera.Application.Responses;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Application.Mapper;

public static class StoreMapper
{
    public static StoreResponse ToResponse(this Store store)
    {
        var newAddress = store.Address is null ? null : new AddressResponse(
            store.Address.Cep,
            store.Address.Street,
            store.Address.Neighborhood,
            store.Address.City,
            store.Address.StateCode,
            store.Address.State,
            store.Address.Complement
        );

        var productTypes = store.ProductTypes?
            .Select(productTypeItem => new ProductTypeResponse(
                productTypeItem.Key,
                productTypeItem.Label,
                productTypeItem.Category
            )).ToList() ?? new List<ProductTypeResponse>();

        return new StoreResponse(
            store.Id,
            store.OwnerUserId,
            store.Name,
            store.AveragePrice,
            store.LocalType,
            productTypes,
            store.AverageQuantity,
            store.Description,
            newAddress,
            store.Reviews,
            store.Phones,
            store.SocialMedia
        );
    }

    public static IReadOnlyList<StoreResponse> ToResponseList(this IReadOnlyList<Store> stores)
    {
        if (stores is null)
            return new List<StoreResponse>();

        return stores.Select(s => ToResponse(s)).ToList();
    }
}
