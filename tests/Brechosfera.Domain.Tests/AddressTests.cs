using Brechosfera.Domain.Exceptions;
using Brechosfera.Tests.Common.Factories;

namespace Brechosfera.Domain.Tests;

public class AddressTests
{
    [Fact]
    public void Constructor_WhenComplementIsNull_CreatesAddress()
    {
        var address = AddressFactory.Create(complement: null);

        Assert.Null(address.Complement);
        Assert.Equal("01001000", address.Cep);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Address_WhenEmptyCEP_ThrowsInvalidException(string cep)
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create(cep: cep));

        Assert.Equal("ADDRESS_INVALID_CEP", exception.Code);
    }

    [Fact]
    public void Address_WhenEmptyStreet_ThrowsInvalidException()
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create( street: "" ));

        Assert.Equal("ADDRESS_INVALID_STREET", exception.Code);
    }

    [Fact]
    public void Address_WhenEmptyNeighborhood_ThrowsInvalidException()
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create( neighborhood: "" ));

        Assert.Equal("ADDRESS_INVALID_NEIGHBORHOOD", exception.Code);
    }

    [Fact]
    public void Address_WhenEmptyCity_ThrowsInvalidException()
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create( city: "" ));

        Assert.Equal("ADDRESS_INVALID_CITY", exception.Code);
    }

    [Fact]
    public void Address_WhenEmptyStateCode_ThrowsInvalidException()
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create( stateCode: "" ));

        Assert.Equal("ADDRESS_INVALID_UF", exception.Code);
    }

    [Fact]
    public void Address_WhenEmptyState_ThrowsInvalidException()
    {
        var exception = Assert.Throws<DomainException>(() => AddressFactory.Create( state: "" ));

        Assert.Equal("ADDRESS_INVALID_STATE", exception.Code);
    }
}
