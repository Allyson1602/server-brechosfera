using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<CreateRegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(100).WithMessage("E-mail muito grande.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Informe o nome de usuário.")
            .MinimumLength(3).WithMessage("Pelo menos 3 caracteres.")
            .MaximumLength(100).WithMessage("No máximo 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Informe a senha.")
            .MaximumLength(100).WithMessage("Senha deve ser menor.");
    }
}
