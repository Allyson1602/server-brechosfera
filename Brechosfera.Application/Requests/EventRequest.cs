namespace Brechosfera.Application.Requests;

public sealed record EventRequest(
    string Banner,
    string Name,
    string Description,
    string OrganizerName,
    AddressRequest Address,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid ResponsibleStoreId
);
