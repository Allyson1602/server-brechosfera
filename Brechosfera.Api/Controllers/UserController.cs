using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[Authorize]
[Route("api/user")]
public class UserController : ApiController
{
    private readonly IUserCredentialService _userService;

    public UserController(INotifier notifier, IUserCredentialService userService) : base(notifier)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] CreateRegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!IsValidOperation())
            return NotificationProblem();

        return Ok(result);
    }
}
