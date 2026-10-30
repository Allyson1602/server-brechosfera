using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;

namespace Brechosfera.Application.Tests.Factories;

public static class StoreFactory
{
    public static Store Create(Guid? ownerUserId = null, string name = "Brechó Teste") => new(
        ownerUserId: ownerUserId ?? Guid.NewGuid(),
        name: name,
        averagePrice: 50m,
        localType: ELocalType.Physical,
        productTypes: [new ProductType("camisetas", "Camisetas", EProductTypeCategory.Top)],
        description: null,
        averageQuantity: null
    );
}
