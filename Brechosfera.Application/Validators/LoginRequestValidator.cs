using Brechosfera.Application.Requests;
using FluentValidation;

namespace Brechosfera.Application.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().WithMessage("Informe o usuário ou e-mail.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Informe a senha.");
    }
}
