using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;

namespace Brechosfera.Application.Interfaces;

public interface IEventService
{
    public Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken);
    public Task<EventResponse?> UpdateEventAsync(Guid id, UpdateEventRequest request, CancellationToken cancellationToken);
    public Task<IReadOnlyList<EventResponse>> ListEventAsync(CancellationToken cancellationToken);
    public Task<IReadOnlyList<EventResponse>> ListEventByStoreIdAsync(Guid storeId, CancellationToken cancellationToken);
    public Task<EventResponse?> CreateEventAsync(CreateEventRequest request, CancellationToken cancellationToken);
}
