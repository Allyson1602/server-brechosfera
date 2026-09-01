using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class PhoneRequestValidator : AbstractValidator<PhoneRequest>
{
    public PhoneRequestValidator()
    {
        RuleFor(x => x.IsPrimary);

        RuleFor(x => x.Number);
    }
}
