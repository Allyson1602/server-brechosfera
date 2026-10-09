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

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _eventService.DeleteEventAsync(id, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventResponse>> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var result = await _eventService.UpdateEventAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponse>>> ListEvent(CancellationToken cancellationToken)
    {
        var result = await _eventService.ListEventAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("store/{storeId:guid}")]
    public async Task<ActionResult<IReadOnlyList<EventResponse>>> ListEventByStoreId(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _eventService.ListEventByStoreIdAsync(storeId, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EventResponse>> CreateEvent(CreateEventRequest request, CancellationToken cancellationToken)
    {
        var result = await _eventService.CreateEventAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Created($"/api/event/{result?.Id}", result);
    }
}
