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

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:guid}/email")]
    public async Task<ActionResult<bool>> UpdateEmail(Guid id, [FromBody] UpdateEmailRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateEmail(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:guid}/password")]
    public async Task<ActionResult<bool>> UpdatePassword(Guid id, [FromBody] UpdatePasswordRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdatePassword(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:guid}/username")]
    public async Task<ActionResult<bool>> UpdateUsername(Guid id, [FromBody] UpdateUsernameRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateUsername(id, request, cancellationToken);

        if (!IsValidOperation())
            return NotificationProblem();

        return NoContent();
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
