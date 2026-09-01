using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class SocialMediaRequestValidator : AbstractValidator<SocialMediaRequest>
{
    public SocialMediaRequestValidator()
    {
        RuleFor(x => x.Link)
            .NotEmpty().WithMessage("Preencha o link.");

        RuleFor(x => x.Platform)
            .IsInEnum().WithMessage("A plataforma da rede social é inválida.");
    }
}
