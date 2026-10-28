using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class StoreTests
{
    private static Store CreateStore(Guid? ownerUserId = null) => new(
        ownerUserId: ownerUserId ?? Guid.NewGuid(),
        name: "Brechó Teste",
        averagePrice: 50m,
        localType: ELocalType.Physical,
        productTypes: [new ProductType("camisetas", "Camisetas", EProductTypeCategory.Top)],
        description: null,
        averageQuantity: null
    );

    [Fact]
    public void AddPhone_WhenLimitIsReached_ThrowsDomainException()
    {
        var store = CreateStore();
        store.AddPhone(new Phone("11999990001"));
        store.AddPhone(new Phone("11999990002"));
        store.AddPhone(new Phone("11999990003"));

        var exception = Assert.Throws<DomainException>(() => store.AddPhone(new Phone("11999990004")));

        Assert.Equal("STORE_PHONE_LIMIT_EXCEEDED", exception.Code);
        Assert.Equal(3, store.Phones.Count);
    }

    [Fact]
    public void AddPhone_WhenFirstPhone_BecomesPrimary()
    {
        var store = CreateStore();

        store.AddPhone(new Phone("11999990001", isPrimary: false));

        Assert.True(store.Phones.Single().IsPrimary);
    }
}
