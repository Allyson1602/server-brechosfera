
using Brechosfera.Domain.Exceptions;

namespace Brechosfera.Domain.Guards;

public static class Guard
{
    public static void AgainstInvalidDateRange(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string code,
        string message
    ){
        if (startDate >= endDate)
            throw new DomainException(code, message);
    }

    public static void AgainstInvalidEnum<TEnum>(TEnum value, string code, string message) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
            throw new DomainException(code, message);
    }

    public static void AgainstNonDigits(string? value, string code, string message)
    {
        if (!string.IsNullOrEmpty(value) && !value.All(char.IsDigit))
            throw new DomainException(code, message);
    }

    public static void AgainstNull(object? value, string code, string message)
    {
        if (value is null)
            throw new DomainException(code, message);
    }

    public static void AgainstEmptyGuid(Guid? value, string code, string message)
    {
        if (!value.HasValue || value.Value == Guid.Empty)
            throw new DomainException(code, message);
    }

    public static void AgainstNullOrWhiteSpace(string? value, string code, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(code, message);
    }

    public static void AgainstNegativeOrZero(decimal value, string code, string message)
    {
        if (value <= 0)
            throw new DomainException(code, message);
    }

    public static void AgainstNegativeOrZero(int? value, string code, string message)
    {
        if (value.HasValue && value <= 0)
            throw new DomainException(code, message);
    }

    public static void AgainstEmptyList<T>(List<T>? list, string code, string message)
    {
        if (list is null || list.Count == 0)
            throw new DomainException(code, message);
    }

    public static void AgainstRange(string value, int min, int max, string code, string message)
    {
        if (value.Length < min || value.Length > max)
           throw new DomainException(code, message);
    }

    public static void AgainstRange(int value, int min, int max, string code, string message)
    {
        if (value < min || value> max)
            throw new DomainException(code, message);
    }

    public static void AgainstNullMaxLength(string? value, int max, string code, string message)
    {
        if (value is null || value.Length > max)
            throw new DomainException(code, message);
    }

    public static void AgainstMaxCount(int currentCount, int max, string code, string message)
    {
        if (currentCount > max)
            throw new DomainException(code, message);
    }

    public static void AgainstSpaces(string value, string code, string message)
    {
        if (value.Contains(" "))
            throw new DomainException(code, message);
    }
}