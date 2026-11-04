using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Tests.Common.Factories;

public static class EventFactory
{
    public static readonly DateTimeOffset BaseDate = new(2026, 11, 1, 10, 0, 0, TimeSpan.FromHours(-3));

    public static Event Create(
        Guid? responsibleStoreId = null,
        string banner = "bannerembase64",
        string name = "Bazar de Primavera",
        string description = "Peças selecionadas com até 50% de desconto.",
        string organizerName = "Ana Souza",
        Address? address = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null
    ) => new(
        banner: banner,
        name: name,
        description: description,
        organizerName: organizerName,
        address: address ?? AddressFactory.Create(),
        startDate: startDate ?? BaseDate,
        endDate: endDate ?? BaseDate.AddHours(4),
        responsibleStoreId: responsibleStoreId ?? Guid.NewGuid()
    );
}
