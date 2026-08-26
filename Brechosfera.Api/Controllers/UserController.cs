using Brechosfera.Application.Common;
using Brechosfera.Application.Interfaces;
using Brechosfera.Application.Notifications;
using Brechosfera.Application.Requests;
using Brechosfera.Application.Responses;
using Brechosfera.Application.Services;
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
    public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return CustomResponse(result);
    }
}
