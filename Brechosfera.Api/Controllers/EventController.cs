using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
[Route("api/event")]
public class EventController : ApiController
{
    private readonly IEventService _eventService;

    public EventController(INotifier notifier, IEventService eventService) : base(notifier)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EventResponse>>>> ListEvent(CancellationToken cancellationToken)
    {
        var result = await _eventService.ListEventAsync(cancellationToken);

        return CustomResponse(result);
    }

    [HttpGet("store/{storeId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EventResponse>>>> ListEventByStoreId(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _eventService.ListEventByStoreIdAsync(storeId, cancellationToken);

        return CustomResponse(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EventResponse>>> CreateEvent(EventRequest request, CancellationToken cancellationToken)
    {
        var result = await _eventService.CreateEventAsync(request, cancellationToken);

        return CustomCreate($"/api/event/{result?.Id}", result);
    }
}
