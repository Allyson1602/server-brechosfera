namespace Brechosfera.Application.Requests;

public sealed record UpdateEventRequest(
    string Banner,
    string Name,
    string Description,
    string OrganizerName,
    AddressRequest Address,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
);
