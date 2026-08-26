using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Application.Services;
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

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var token = await _userService.LoginAsync(request);
        return CustomResponse(token);
    }
}
