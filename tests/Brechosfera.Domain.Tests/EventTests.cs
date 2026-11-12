using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Exceptions;
using Brechosfera.Tests.Common.Factories;

namespace Brechosfera.Domain.Tests;

public class EventTests
{
    private static readonly DateTimeOffset BaseDate = EventFactory.BaseDate;

    [Fact]
    public void UpdateEvent_WhenValidData_UpdatesAllFieldsExceptStore()
    {

        var storeId = Guid.NewGuid();
        var storeEvent = EventFactory.Create(responsibleStoreId: storeId);
        var newAddress = AddressFactory.Create(street: "Avenida Paulista", neighborhood: "Bela Vista");
        var newStart = BaseDate.AddDays(7);

        storeEvent.UpdateEvent(
            banner: "novobanner",
            name: "Bazar de Verão",
            description: "Nova descrição.",
            organizerName: "Bruno Lima",
            address: newAddress,
            startDate: newStart,
            endDate: newStart.AddHours(6)
        );

        Assert.Equal("novobanner", storeEvent.Banner);
        Assert.Equal("Bazar de Verão", storeEvent.Name);
        Assert.Equal("Nova descrição.", storeEvent.Description);
        Assert.Equal("Bruno Lima", storeEvent.OrganizerName);
        Assert.Equal(newAddress, storeEvent.Address);
        Assert.Equal(newStart, storeEvent.StartDate);
        Assert.Equal(newStart.AddHours(6), storeEvent.EndDate);
        Assert.Equal(storeId, storeEvent.ResponsibleStoreId);
    }

    [Fact]
    public void UpdateEvent_WhenDateRangeIsInvalid_DoesNotChangeEvent()
    {
        var storeEvent = EventFactory.Create(name: "Bazar de Primavera");
        var originalStart = storeEvent.StartDate;
        var originalEnd = storeEvent.EndDate;

        var exception = Assert.Throws<DomainException>(() => storeEvent.UpdateEvent(
            banner: "novobanner",
            name: "Bazar de Verão",
            description: "Nova descrição.",
            organizerName: "Bruno Lima",
            address: AddressFactory.Create(),
            startDate: BaseDate,
            endDate: BaseDate.AddHours(-1)
        ));

        Assert.Equal("DATE_RANGE_INVALID", exception.Code);
        Assert.Equal("Bazar de Primavera", storeEvent.Name);
        Assert.Equal(originalStart, storeEvent.StartDate);
        Assert.Equal(originalEnd, storeEvent.EndDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateEvent_WhenDateRange_ThrowsDomainException(int endOffsetInHours)
    {
        var newEvent = EventFactory.Create();
        var start = BaseDate;
        var end = BaseDate.AddHours(endOffsetInHours);

        var exception = Assert.Throws<DomainException>(() => newEvent.UpdateEvent(
            banner: newEvent.Banner,
            name: newEvent.Name,
            description: newEvent.Description,
            organizerName: newEvent.OrganizerName,
            address: newEvent.Address,
            startDate: start,
            endDate: end
        ));

        Assert.Equal("DATE_RANGE_INVALID", exception.Code);
    }

    [Fact]
    public void Constructor_WhenValidData_CreatesEvent()
    {
        var storeId = Guid.NewGuid();
        var address = AddressFactory.Create();

        var storeEvent = new Event(
            banner: "bannerembase64",
            name: "Bazar de Primavera",
            description: "Peças selecionadas.",
            organizerName: "Ana Souza",
            address: address,
            startDate: BaseDate,
            endDate: BaseDate.AddHours(4),
            responsibleStoreId: storeId
        );

        Assert.Equal("Bazar de Primavera", storeEvent.Name);
        Assert.Equal("Ana Souza", storeEvent.OrganizerName);
        Assert.Equal(address, storeEvent.Address);
        Assert.Equal(BaseDate, storeEvent.StartDate);
        Assert.Equal(BaseDate.AddHours(4), storeEvent.EndDate);
        Assert.Equal(storeId, storeEvent.ResponsibleStoreId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenBannerIsBlank_ThrowsDomainException(string banner)
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(banner: banner));

        Assert.Equal("BANNER_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsBlank_ThrowsDomainException(string name)
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(name: name));

        Assert.Equal("NAME_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenDescriptionIsBlank_ThrowsDomainException(string description)
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(description: description));

        Assert.Equal("DESCRIPTION_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenOrganizerNameIsBlank_ThrowsDomainException(string organizerName)
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(organizerName: organizerName));

        Assert.Equal("ORGANIZER_NAME_EMPTY", exception.Code);
    }

    [Fact]
    public void Constructor_WhenAddressIsNull_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new Event(
            banner: "bannerembase64",
            name: "Bazar de Primavera",
            description: "Peças selecionadas.",
            organizerName: "Ana Souza",
            address: null!,
            startDate: BaseDate,
            endDate: BaseDate.AddHours(4),
            responsibleStoreId: Guid.NewGuid()
        ));

        Assert.Equal("EVENT_ADDRESS_EMPTY", exception.Code);
    }

    [Fact]
    public void Constructor_WhenResponsibleStoreIsEmpty_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(responsibleStoreId: Guid.Empty));

        Assert.Equal("RESPONSIBLE_STORE_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenEndIsNotAfterStart_ThrowsDomainException(int endOffsetInHours)
    {
        var exception = Assert.Throws<DomainException>(() => EventFactory.Create(
            startDate: BaseDate,
            endDate: BaseDate.AddHours(endOffsetInHours)
        ));

        Assert.Equal("DATE_RANGE_INVALID", exception.Code);
    }
}
