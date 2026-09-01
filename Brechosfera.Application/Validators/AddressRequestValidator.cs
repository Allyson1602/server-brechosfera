using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("Preencha o CEP.")
            .MinimumLength(8).WithMessage("CEP deve ter 8 dígitos")
            .MaximumLength(8).WithMessage("CEP deve ter 8 dígitos");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Preencha a rua.");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("Preencha o bairro.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Preencha a cidade.");

        RuleFor(x => x.StateCode)
            .NotEmpty().WithMessage("Preencha o código do estado.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("Preencha o estado.");

        RuleFor(x => x.Complement)
            .MaximumLength(150).WithMessage("Complemento com no máximo 150 caracteres.");
    }
}
