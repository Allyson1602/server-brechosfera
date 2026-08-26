using FluentValidation;
using Brechosfera.Application.Requests;

namespace Brechosfera.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Preencha todos os campos.")
            .MinimumLength(3).WithMessage("Pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("No máximo 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Preencha todos os campos.")
            .MaximumLength(100).WithMessage("Senha deve ser menor.");
    }
}
