using Brechosfera.Application.Common;
using Brechosfera.Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    private readonly INotifier _notifier;

    protected ApiController(INotifier notifier)
    {
        _notifier = notifier;
    }

    protected bool IsValidOperation() => !_notifier.HasNotifications();

    protected ActionResult<ApiResponse<T>> CustomError<T>()
    {
        return BadRequest(new ApiResponse<T>(
            Success: false,
            Data: default,
            Errors: _notifier.GetNotifications().Select(x => new ApiError(x.Code, x.Message)).ToArray()
        ));
    }

    protected ActionResult<ApiResponse<T>> CustomResponse<T>(T? result = default)
    {
        if (!IsValidOperation())
            return CustomError<T>();

        return Ok(ApiResponse<T>.Ok(result));
    }

    protected ActionResult<ApiResponse<T>> CustomCreate<T>(string uri, T? result)
    {
        if (!IsValidOperation())
            return CustomError<T>();

        return Created(uri, ApiResponse<T>.Ok(result));
    }

    protected ActionResult<ApiResponse<object>> CustomNoContent()
    {
        if (!IsValidOperation())
            return CustomError<object>();

        return NoContent();
    }
}
