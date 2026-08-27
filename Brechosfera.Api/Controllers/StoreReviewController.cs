using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
[Route("api/store-review")]
public class StoreReviewController : ApiController
{
    private readonly IStoreReviewService _storeReviewService;

    public StoreReviewController(
        INotifier notifier,
        IStoreReviewService storeReviewService
    ) : base(notifier) {
        _storeReviewService = storeReviewService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StoreReviewResponse>>>> ListStoreReviewByStoreId(
        [FromQuery] Guid storeId,
        CancellationToken cancellationToken
    ) {
        var storeReviews = await _storeReviewService.ListStoreReviewByStoreIdAsync(storeId, cancellationToken);

        return CustomResponse(storeReviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StoreReviewResponse>>> GetStoreReviewById(Guid id, CancellationToken cancellationToken)
    {
        var storeReview = await _storeReviewService.GetStoreReviewByIdAsync(id, cancellationToken);

        return CustomResponse(storeReview);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StoreReviewResponse>>> AddStoreReview([FromBody] StoreReviewRequest request, CancellationToken cancellationToken)
    {
        var storeReview = await _storeReviewService.AddStoreReviewAsync(request, cancellationToken);

        return CustomCreate($"/api/store-review/{storeReview?.Id}", storeReview);
    }
}
