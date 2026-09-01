using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
[Route("api/store")]
public class StoreController : ApiController
{
    private readonly IStoreService _storeService;

    public StoreController(INotifier notifier, IStoreService storeService) : base(notifier)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreResponse>>> ListStore(CancellationToken cancellationToken)
    {
        var result = await _storeService.ListStoreAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreResponse>> GetStoreById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storeService.GetStoreByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<StoreResponse>> CreateStore(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _storeService.CreateStoreAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Created($"/api/store/{result?.Id}", result);
    }
}
