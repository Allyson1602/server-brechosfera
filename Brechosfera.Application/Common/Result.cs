namespace Brechosfera.Application.Common;

public class Result
{
    public bool Succeeded { get; init; }
    public string[] Errors { get; init; } = [];

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}
