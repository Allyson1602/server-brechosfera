using Brechosfera.Application.Common;
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
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StoreResponse>>>> ListStore(CancellationToken cancellationToken)
    {
        var result = await _storeService.ListStoreAsync(cancellationToken);

        return CustomResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StoreResponse>>> GetStoreById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storeService.GetStoreByIdAsync(id, cancellationToken);

        return CustomResponse(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StoreResponse>>> CreateStore(StoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _storeService.CreateStoreAsync(request, cancellationToken);

        return CustomCreate($"/api/store/{result?.Id}", result);
    }
}
