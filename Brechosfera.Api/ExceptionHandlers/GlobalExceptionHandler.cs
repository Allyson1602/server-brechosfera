using Brechosfera.Application.Common;
using Microsoft.AspNetCore.Diagnostics;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unexpected error while processing the request.");

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        var response = ApiResponse<object>.Fail(
            new ApiError(
                "INTERNAL_SERVER_ERROR",
                "Ocorreu um erro inesperado. Tente novamente mais tarde."
            )
        );

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}