namespace Brechosfera.Application.Requests;

public sealed record CreateEventRequest(
    string Banner,
    string Name,
    string Description,
    string OrganizerName,
    AddressRequest Address,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid ResponsibleStoreId
);
