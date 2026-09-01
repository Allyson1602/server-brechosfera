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
    public async Task<ActionResult<List<StoreReviewResponse>>> ListStoreReviewByStoreId(
        [FromQuery] Guid storeId,
        CancellationToken cancellationToken
    ) {
        var storeReviews = await _storeReviewService.ListStoreReviewByStoreIdAsync(storeId, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(storeReviews);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreReviewResponse>> GetStoreReviewById(Guid id, CancellationToken cancellationToken)
    {
        var storeReview = await _storeReviewService.GetStoreReviewByIdAsync(id, cancellationToken);

        if (storeReview is null)
            return NotFound();

        return Ok(storeReview);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<StoreReviewResponse>> AddStoreReview([FromBody] CreateStoreReviewRequest request, CancellationToken cancellationToken)
    {
        var storeReview = await _storeReviewService.AddStoreReviewAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Created($"/api/store-review/{storeReview?.Id}", storeReview);
    }
}
