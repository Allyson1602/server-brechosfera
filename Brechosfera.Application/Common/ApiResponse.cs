namespace Brechosfera.Application.Common;

public sealed record ApiError(
    string Code,
    string Message
);

public sealed record ApiResponse<T>(
  bool Success,
  T? Data,
  IEnumerable<ApiError>? Errors
)
{
    public static ApiResponse<T> Ok(T? data) => new(true, data, null);

    public static ApiResponse<T> Fail(params ApiError[] errors) => new(false, default, errors);
}
