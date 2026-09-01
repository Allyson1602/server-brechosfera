using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Banner)
            .NotEmpty().WithMessage("Preencha o banner.");

        RuleFor(x => x.Name)
            .MaximumLength(150).WithMessage("Nome com no máximo 150 caracteres.")
            .NotEmpty().WithMessage("Preencha o nome.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Nome com no máximo 500 caracteres.")
            .NotEmpty().WithMessage("Preencha a descrição.");

        RuleFor(x => x.OrganizerName)
            .MaximumLength(150).WithMessage("Nome do responsável com no máximo 150 caracteres.")
            .NotEmpty().WithMessage("Preencha o nome do responsável.");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Preencha o endereço.")
            .SetValidator(new AddressRequestValidator());

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Preencha a data de início.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .NotEmpty().WithMessage("Preencha a data de término.");

        RuleFor(x => x.ResponsibleStoreId).NotEmpty();
    }
}
