using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
    public UpdateStoreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Preencha o nome da loja.")
            .MaximumLength(150).WithMessage("Nome com no máximo 150 caracteres.")
            .When(x => x.Name is not null);

        RuleFor(x => x.AveragePrice)
            .GreaterThan(0).WithMessage("Preço médio deve ser maior que zero.");

        RuleFor(x => x.LocalType)
            .IsInEnum().WithMessage("Tipo do local deve ser válido.");

        RuleFor(x => x.AverageQuantity)
            .GreaterThan(0).WithMessage("Preencha o a quantidade média.")
            .When(x => x.AverageQuantity.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição com no máximo 500 caracteres.")
            .When(x => x.Description is not null);
    }
}
