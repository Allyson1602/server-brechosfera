using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.Guards;

namespace Brechosfera.Domain.ValueObjects;

public sealed record Rating
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Rating Create(int value)
    {
        Guard.AgainstRange(value, 1, 5, "RATING_OUT_RANGE", "A avaliação deve estar entre 1 e 5.");

        return new Rating(value);
    }
}
