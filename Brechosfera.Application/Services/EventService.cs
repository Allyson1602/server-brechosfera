using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Responses;
using Brechosfera.Application.Mapper;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Repositories;
using Brechosfera.Application.Requests;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Application.Services;

public class EventService : BaseService, IEventService
{
    private readonly IEventRepository _repository;
    private readonly IStoreRepository _storeRepository;

    public EventService(
        IUnitOfWork unitOfWork,
        INotifier notifier,
        IEventRepository repository,
        IStoreRepository storeRepository
    ) : base(unitOfWork, notifier) {
        _repository = repository;
        _storeRepository = storeRepository;
    }

    public async Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken)
    {
        var eventData = await _repository.GetByIdAsync(id);

        if (eventData is null)
        {
            Notify(
                "EventId",
                "O evento informado não foi encontrado."
            );

            return false;
        }

        if (eventData.DeletedAt.HasValue)
        {
            Notify(
                "DeleteEvent",
                "O evento já foi deletado."
            );

            return false;
        }

        eventData.MarkAsDeleted();

        var success = await CommitAsync(cancellationToken);

        return success;
    }

    public async Task<EventResponse?> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var eventData = await _repository.GetByIdAsync(id);

        if (eventData is null)
        {
            Notify(
                "EventId",
                "O evento informado não foi encontrado."
            );

            return null;
        }

        var newAddress = new Address(
            request.Address.Cep,
            request.Address.Street,
            request.Address.Neighborhood,
            request.Address.City,
            request.Address.StateCode,
            request.Address.State,
            request.Address.Complement
        );

        eventData.UpdateEvent(
            request.Banner,
            request.Name,
            request.Description,
            request.OrganizerName,
            newAddress,
            request.StartDate,
            request.EndDate
        );

        eventData.MarkAsUpdated();

        var success = await CommitAsync(cancellationToken);

        return success ? eventData.ToResponse() : null;
    }

    public async Task<IReadOnlyList<EventResponse>> ListEventAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Event> events = await _repository.GetAllAsync(cancellationToken);

        return events.ToResponseList();
    }

    public async Task<IReadOnlyList<EventResponse>> ListEventByStoreIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        Store? store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);

        if (store is null)
        {
            Notify(
                "StoreId",
                "A loja informada não foi encontrada."
            );

            return [];
        }

        IReadOnlyList<Event> events = await _repository.ListByStoreIdAsync(store.Id, cancellationToken);

        return events.ToResponseList();
    }

    public async Task<EventResponse?> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(request.ResponsibleStoreId);

        if (store is null)
        {
            Notify(
                "ResponsibleStoreId",
                "A loja informada não foi encontrada."
            );

            return null;
        }

        Address newAddress = new Address(
            request.Address.Cep,
            request.Address.Street,
            request.Address.Neighborhood,
            request.Address.City,
            request.Address.StateCode,
            request.Address.State,
            request.Address.Complement
        );

        Event newEvent = new Event(
            request.Banner,
            request.Name,
            request.Description,
            request.OrganizerName,
            newAddress,
            request.StartDate,
            request.EndDate,
            request.ResponsibleStoreId
        );

        _repository.Add(newEvent);

        var success = await CommitAsync(cancellationToken);

        return success ? newEvent.ToResponse() : null;
    }
}
