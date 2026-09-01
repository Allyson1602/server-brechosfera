using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly IUserCredentialService _userService;

    public AuthController(INotifier notifier, IUserCredentialService userService) : base(notifier)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] CreateLoginRequest request)
    {
        var token = await _userService.LoginAsync(request);

        if (!IsValidOperation())
        {
            var message = _notifier
                .GetNotifications()
                .FirstOrDefault()?.Message;

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Falha na autenticação",
                detail: message
            );
        }

        return Ok(token);
    }
}
