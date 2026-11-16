using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class AddStoreProductTypeRequestValidator : AbstractValidator<ReplaceStoreProductTypesRequest>
{
    public AddStoreProductTypeRequestValidator()
    {
        RuleFor(x => x.ProductTypeIds)
            .NotEmpty().WithMessage("O tipo de produto é inválido.");
    }
}
