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

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteStore(Guid id, CancellationToken cancellationToken)
    {
        await _storeService.DeleteStoreAsync(id, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpDelete("{id:guid}/social-media")]
    public async Task<ActionResult> RemoveSocialMedia(Guid id, [FromBody]RemoveSocialMediaRequest request, CancellationToken cancellationToken)
    {
        await _storeService.RemoveSocialMediaAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpPost("{id:guid}/social-media")]
    public async Task<ActionResult> AddSocialMedia(Guid id, [FromBody]SocialMediaRequest request, CancellationToken cancellationToken)
    {
        await _storeService.AddSocialMediaAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpDelete("{id:guid}/phone")]
    public async Task<ActionResult> RemovePhone(Guid id, [FromBody]RemovePhoneRequest request, CancellationToken cancellationToken)
    {
        await _storeService.RemovePhoneAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpPost("{id:guid}/phone")]
    public async Task<ActionResult> AddPhone(Guid id, [FromBody]PhoneRequest request, CancellationToken cancellationToken)
    {
        await _storeService.AddPhoneAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpPut("{id:guid}/address")]
    public async Task<ActionResult> UpdateAddress(Guid id, [FromBody]AddressRequest request, CancellationToken cancellationToken)
    {
        await _storeService.UpdateAddressAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpPut("{id:guid}/product-types")]
    public async Task<ActionResult> UpdateProductTypes(Guid id, [FromBody]ReplaceStoreProductTypesRequest request, CancellationToken cancellationToken)
    {
        await _storeService.UpdateProductTypesAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StoreResponse>> UpdateStore(Guid id, [FromBody]UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _storeService.UpdateStoreAsync(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreResponse>>> ListStore(CancellationToken cancellationToken)
    {
        var result = await _storeService.ListStoreAsync(cancellationToken);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreResponse>> GetStoreById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _storeService.GetStoreByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<StoreResponse>> CreateStore(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _storeService.CreateStoreAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Created($"/api/store/{result?.Id}", result);
    }
}
