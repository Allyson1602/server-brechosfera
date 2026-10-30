using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class StoreTests
{
    private static Store CreateStore(
        Guid? ownerUserId = null,
        string name = "Brechó Teste",
        decimal averagePrice = 50m
    ) => new(
        ownerUserId: ownerUserId ?? Guid.NewGuid(),
        name: name,
        averagePrice: averagePrice,
        localType: ELocalType.Physical,
        productTypes: [new ProductType("camisetas", "Camisetas", EProductTypeCategory.Top)],
        description: null,
        averageQuantity: null
    );

    [Fact]
    public void AddSocialMedia_WhenDuplicatedItems_AddsOnlyOne()
    {
        var store = CreateStore();
        var socialMedia = new SocialMedia("https://instagram.com/brechoteste", ESocialMediaPlatform.Instagram);

        store.AddSocialMedia(socialMedia);
        store.AddSocialMedia(socialMedia);

        Assert.Single(store.SocialMedia);
    }

    [Fact]
    public void RemoveSocialMedia_WhenIsTheLastOne_ThrowsDomainException()
    {
        var store = CreateStore();
        store.AddSocialMedia(new SocialMedia("https://instagram.com/brechoteste", ESocialMediaPlatform.Instagram));

        var exception = Assert.Throws<DomainException>(
            () => store.RemoveSocialMedia("https://instagram.com/brechoteste")
        );

        Assert.Equal("SOCIAL_MEDIA_MIN", exception.Code);
    }

    [Fact]
    public void Constructor_WhenLocalTypeIsInvalid_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Store(
            ownerUserId: Guid.NewGuid(),
            name: "Brechó Teste",
            averagePrice: 50m,
            localType: (ELocalType)999,   // um valor que não existe no enum
            productTypes: [new ProductType("camisetas", "Camisetas", EProductTypeCategory.Top)],
            description: null,
            averageQuantity: null
        ));

        Assert.Equal("STORE_LOCAL_TYPE_INVALID", exception.Code);
    }

    [Fact]
    public void UpdateDetails_WhenNameIsBlank_DoesNotChangeStore()
    {
        var store = CreateStore();
        var originalName = store.Name;
        var originalPrice = store.AveragePrice;

        Assert.Throws<DomainException>(() => store.UpdateDetails(
            name: "   ",
            averagePrice: 999m,
            localType: ELocalType.Online,
            averageQuantity: null,
            description: "Nova descrição"
        ));

        Assert.Equal(originalName, store.Name);
        Assert.Equal(originalPrice, store.AveragePrice);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesAllFields()
    {
        var store = CreateStore();

        store.UpdateDetails(
            name: "Brechó Renovado",
            averagePrice: 999m,
            localType: ELocalType.Online,
            averageQuantity: 20,
            description: "Nova descrição"
        );

        Assert.Equal("Brechó Renovado", store.Name);
        Assert.Equal(999m, store.AveragePrice);
        Assert.Equal(ELocalType.Online, store.LocalType);
        Assert.Equal(20, store.AverageQuantity);
        Assert.Equal("Nova descrição", store.Description);
    }

    [Fact]
    public void ReplaceProductTypes_WithNewList_ReplacesAllProductTypes()
    {
        var store = CreateStore();
        var productType = new ProductType("maiô", "Maiô", EProductTypeCategory.Beachwear);

        store.ReplaceProductTypes([productType]);

        Assert.Single(store.ProductTypes);
        Assert.Contains(productType, store.ProductTypes);
    }

    [Fact]
    public void ReplaceProductTypes_WithDuplicatedItems_AddsOnlyOnce()
    {
        var store = CreateStore();
        var productType = new ProductType("maiô", "Maiô", EProductTypeCategory.Beachwear);

        store.ReplaceProductTypes([productType, productType]);

        Assert.Single(store.ProductTypes);
        Assert.True(store.ProductTypes.Contains(productType) == true);
    }

    [Fact]
    public void ReplaceProductTypes_WhenListIsEmpty_KeepsCurrentProductTypes()
    {
        var store = CreateStore();

        Assert.Throws<DomainException>(() => store.ReplaceProductTypes([]));
        Assert.Single(store.ProductTypes);
    }

    [Fact]
    public void CheckIsOwnedBy_WhenUserIsOwner_ReturnsTrue()
    {
        var ownerId = Guid.NewGuid();
        var store = CreateStore(ownerId);

        Assert.True(store.CheckIsOwnedBy(ownerId));
    }

    [Fact]
    public void CheckIsOwnedBy_WhenUserIsNotOwner_ReturnsFalse()
    {
        var store = CreateStore(Guid.NewGuid());

        Assert.False(store.CheckIsOwnedBy(Guid.NewGuid()));
    }

    [Fact]
    public void UpdatePrimaryPhone_WhenPhoneDoesNotExist_ThrowsDomainException()
    {
        var store = CreateStore();

        store.AddListPhone([new Phone("11999990001"), new Phone("11999990002")]);

        var exception = Assert.Throws<DomainException>(() => store.UpdatePrimaryPhone("11999990003"));

        Assert.Equal("PHONE_NOT_FOUND", exception.Code);
    }

    [Fact]
    public void UpdatePrimaryPhone_WhenPhoneIsAlreadyPrimary_KeepsPhonesUnchanged()
    {
        var store = CreateStore();
        store.AddPhone(new Phone("11999990001"));
        store.AddPhone(new Phone("11999990002"));

        store.UpdatePrimaryPhone("11999990001");

        Assert.Equal(2, store.Phones.Count);
        Assert.True(store.Phones.Single(p => p.Number == "11999990001").IsPrimary);
        Assert.False(store.Phones.Single(p => p.Number == "11999990002").IsPrimary);
    }

    [Fact]
    public void UpdatePrimaryPhone_WhenPhoneExists_BecomesTheOnlyPrimary()
    {
        var store = CreateStore();
        store.AddListPhone([new Phone("11999990001", true), new Phone("11999990002")]);

        store.UpdatePrimaryPhone("11999990002");

        Assert.Single(store.Phones, p => p.IsPrimary);
        Assert.True(store.Phones.Single(p => p.Number == "11999990002").IsPrimary);
        Assert.Equal(2, store.Phones.Count);
    }

    [Fact]
    public void AddPhone_WhenNumberAlreadyExists_ThrowsDomainException()
    {
        var store = CreateStore();
        store.AddPhone(new Phone("11999990001"));

        var exception = Assert.Throws<DomainException>(() => store.AddPhone(new Phone("11999990001")));

        Assert.Equal("STORE_PHONE_DUPLICATED", exception.Code);
    }

    [Fact]
    public void AddPhone_WhenNewPhoneIsPrimary_PreviousPrimaryIsUnset()
    {
        var store = CreateStore();
        store.AddPhone(new Phone("11999990001"));

        store.AddPhone(new Phone("11999990002", isPrimary: true));

        Assert.Single(store.Phones, p => p.IsPrimary);
        Assert.True(store.Phones.Single(p => p.Number == "11999990002").IsPrimary);
    }

    [Fact]
    public void RemovePhone_WhenRemovingPrimary_AnotherPhoneBecomesPrimary()
    {
        var store = CreateStore();
        store.AddPhone(new Phone("11999990001"));
        store.AddPhone(new Phone("11999990002"));

        store.RemovePhone("11999990001");

        Assert.True(store.Phones.Single().IsPrimary);
    }

    [Fact]
    public void Constructor_WhenOwnerIsEmpty_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => CreateStore(Guid.Empty));

        Assert.Equal("STORE_OWNER_INVALID", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsBlank_ThrowsDomainException(string invalidName)
    {
        var exception = Assert.Throws<DomainException>(() => CreateStore(name: invalidName));

        Assert.Equal("STORE_NAME_INVALID", exception.Code);
    }

    [Fact]
    public void Constructor_WhenProductTypeIsEmpty_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Store(
                ownerUserId: Guid.NewGuid(),
                name: "Brechó Teste",
                averagePrice: 50m,
                localType: ELocalType.Physical,
                productTypes: [],
                description: null,
                averageQuantity: null
            ));

        Assert.Equal("PRODUCT_TYPE_EMPTY_LIST", exception.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenAveragePriceIsNotPositive_ThrowsDomainException(int invalidPrice)
    {
        var exception = Assert.Throws<DomainException>(() => CreateStore(averagePrice: invalidPrice));

        Assert.Equal("STORE_AVERAGE_PRICE_MORE_ZERO", exception.Code);
    }

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
