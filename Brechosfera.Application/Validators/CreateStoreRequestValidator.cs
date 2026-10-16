using Brechosfera.Application.Requests;
using Brechosfera.Domain.Entities;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class CreateStoreRequestValidator : AbstractValidator<CreateStoreRequest>
{
    public CreateStoreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Preencha o nome da loja.")
            .MaximumLength(150).WithMessage("Nome com no máximo 150 caracteres.");

        RuleFor(x => x.AveragePrice)
            .GreaterThan(0).WithMessage("Preço médio deve ser maior que zero.");

        RuleFor(x => x.LocalType)
            .IsInEnum().WithMessage("Tipo do local deve ser válido.");

        RuleFor(x => x.ProductTypeKeys)
            .NotNull().WithMessage("Preencha os tipos de produtos.")
            .Must(x => x.Any()).WithMessage("Preencha pelo menos um tipo de produto.");

        RuleFor(x => x.AverageQuantity)
            .GreaterThan(0).WithMessage("Preencha o a quantidade média.")
            .When(x => x.AverageQuantity.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição com no máximo 500 caracteres.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Phones)
            .Must(phones =>
                phones is null || phones.Count() <= Store.MaxPhonesLimit)
            .WithMessage($"A loja pode possuir no máximo {Store.MaxPhonesLimit} celulares.");

        RuleFor(x => x.Phones)
            .Must(phones =>
                phones is null || phones.Count(phone => phone.IsPrimary) <= 1
            ).WithMessage("A loja pode possuir apenas um celular principal.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!).SetValidator(new AddressRequestValidator());
        });

        RuleForEach(x => x.SocialMedia)
            .SetValidator(new SocialMediaRequestValidator());

        RuleForEach(x => x.Phones)
            .SetValidator(new PhoneRequestValidator());
    }
}
