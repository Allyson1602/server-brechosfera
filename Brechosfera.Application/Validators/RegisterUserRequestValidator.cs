using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(256);

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Informe o nome de usuário.")
            .Length(3, 30).WithMessage("Entre 3 e 30 caracteres.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Use apenas letras, números, '.', '_' ou '-'.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Informe a senha.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");
    }
}
