using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[AllowAnonymous]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly IAuthService _authService;

    public AuthController(INotifier notifier, IAuthService authService) : base(notifier) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var userId = await _authService.RegisterAsync(request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(new { userId, message = "Cadastro realizado. Enviamos um link de confirmação para o seu e-mail." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(userId, token);

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
