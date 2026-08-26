using Brechosfera.Application.Common;
using Brechosfera.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Brechosfera.Api.ExceptionHandlers;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false;

        httpContext.Response.StatusCode =
            StatusCodes.Status422UnprocessableEntity;

        var response = ApiResponse<object>.Fail(
            new ApiError(
                domainException.Code,
                domainException.Message
            )
        );

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}
