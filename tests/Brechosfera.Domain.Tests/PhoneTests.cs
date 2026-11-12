using Brechosfera.Domain.Exceptions;
using Brechosfera.Domain.ValueObjects;

namespace Brechosfera.Domain.Tests;

public class PhoneTests
{
    [Fact]
    public void Constructor_WithValidNumber_CreatesPhone()
    {
        var phone = new Phone("11999990001", isPrimary: true);

        Assert.Equal("11999990001", phone.Number);
        Assert.True(phone.IsPrimary);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNumberIsBlank_ThrowsDomainException(string phoneNumber)
    {
        var exception = Assert.Throws<DomainException>(() => new Phone(phoneNumber));

        Assert.Equal("PHONE_EMPTY", exception.Code);
    }

    [Theory]
    [InlineData("11 99999-0001")]
    [InlineData("abc")]
    public void Phone_WhenInvalidNumber_ThrowsExceptionOnlyNumbers(string phoneNumber)
    {
        var exception = Assert.Throws<DomainException>(() => new Phone(
            number: phoneNumber
        ));

        Assert.Equal("PHONE_ONLY_NUMBERS", exception.Code);
    }
}
