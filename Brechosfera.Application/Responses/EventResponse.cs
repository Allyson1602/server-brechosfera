using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Responses;

public sealed record EventResponse(
        Guid Id,
        string Banner,
        string Name,
        string Description,
        string OrganizerName,
        AddressResponse Address,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate,
        Guid ResponsibleStoreId
);
