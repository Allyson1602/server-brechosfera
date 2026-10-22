using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Brechosfera.Api.Controllers;

[AllowAnonymous]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly IAuthService _authService;

    public AuthController(INotifier notifier, IAuthService authService) : base(notifier) => _authService = authService;

    [EnableRateLimiting("email-sending")]
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        await _authService.ResendConfirmationAsync(request, cancellationToken);

        return Ok(new { message = "Se houver uma conta aguardando confirmação com esse e-mail, enviaremos um novo link." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var userId = await _authService.RegisterAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(new { userId, message = "Cadastro realizado. Enviamos um link de confirmação para o seu e-mail." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken cancellationToken)
    {
        await _authService.ConfirmEmailAsync(userId, token, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(new { message = "E-mail confirmado. Você já pode entrar." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(response);
    }
}
