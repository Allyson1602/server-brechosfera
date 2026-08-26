using Brechosfera.Application.Responses;
using Brechosfera.Domain.Entities;

namespace Brechosfera.Application.Mapper;

public static class EventMapper
{
    public static EventResponse ToResponse(this Event @event) {
        var newAddress = new AddressResponse(
            @event.Address.Cep,
            @event.Address.Street,
            @event.Address.Neighborhood,
            @event.Address.City,
            @event.Address.StateCode,
            @event.Address.State,
            @event.Address.Complement
        );

        return new EventResponse(
                @event.Id,
                @event.Banner,
                @event.Name,
                @event.Description,
                @event.OrganizerName,
                newAddress,
                @event.StartDate,
                @event.EndDate,
                @event.ResponsibleStoreId
        );
    }

    public static IReadOnlyList<EventResponse> ToResponseList(this IReadOnlyList<Event> events)
    {
        if (events is null)
            return new List<EventResponse>();

        return events.Select(e => e.ToResponse()).ToList();
    }
}
