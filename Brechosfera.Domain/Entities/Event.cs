using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Guards;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Entities;

public class Event : Entity, IAggregateRoot
{
    public string Banner { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string OrganizerName { get; private set; } = string.Empty;
    public Address Address { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public Guid ResponsibleStoreId { get; }

    private Event() { }

    public Event(
        string banner,
        string name,
        string description,
        string organizerName,
        Address address,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid responsibleStoreId
    ){
        Guard.AgainstNullOrWhiteSpace(banner, "BANNER_EMPTY", "O banner não pode ser nulo.");
        Guard.AgainstNullOrWhiteSpace(name, "NAME_EMPTY", "O nome não pode ser nulo.");
        Guard.AgainstNullOrWhiteSpace(description, "DESCRIPTION_EMPTY", "A descrição não pode ser nula.");
        Guard.AgainstNullOrWhiteSpace(organizerName, "ORGANIZER_NAME_EMPTY", "O nome do organizador não pode ser nulo.");
        Guard.AgainstNull(address, "EVENT_ADDRESS_EMPTY", "A localização não pode ser nula.");
        Guard.AgainstEmptyGuid(responsibleStoreId, "RESPONSIBLE_STORE_EMPTY", "A loja responsável não pode ser nula.");
        Guard.AgainstInvalidDateRange(startDate, endDate, "DATE_RANGE_INVALID", "A data de início deve ser anterior à data de término.");

        Banner = banner;
        Name = name;
        Description = description;
        OrganizerName = organizerName;
        Address = address;
        StartDate = startDate;
        EndDate = endDate;
        ResponsibleStoreId = responsibleStoreId;
    }

    public void UpdateEvent(
        string banner,
        string name,
        string description,
        string organizerName,
        Address address,
        DateTimeOffset startDate,
        DateTimeOffset endDate
    )
    {
        Guard.AgainstNullOrWhiteSpace(banner, "BANNER_EMPTY", "O banner não pode ser nulo.");
        Guard.AgainstNullOrWhiteSpace(name, "NAME_EMPTY", "O nome não pode ser nulo.");
        Guard.AgainstNullOrWhiteSpace(description, "DESCRIPTION_EMPTY", "A descrição não pode ser nula.");
        Guard.AgainstNullOrWhiteSpace(organizerName, "ORGANIZER_NAME_EMPTY", "O nome do organizador não pode ser nulo.");
        Guard.AgainstNull(address, "EVENT_ADDRESS_EMPTY", "A localização não pode ser nula.");
        Guard.AgainstInvalidDateRange(startDate, endDate, "DATE_RANGE_INVALID", "A data de início deve ser anterior à data de término.");

        Banner = banner;
        Name = name;
        Description = description;
        OrganizerName = organizerName;
        Address = address;
        StartDate = startDate;
        EndDate = endDate;
    }
}
