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
    private readonly IIdentityService _identity;

    public AuthController(INotifier notifier, IIdentityService identity) : base(notifier) => _identity = identity;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var (result, userId) = await _identity.RegisterAsync(request);

        return result.Succeeded
            ? Ok(new { userId })
            : BadRequest(new { result.Errors });
    }
}
