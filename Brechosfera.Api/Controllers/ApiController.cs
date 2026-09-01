using Brechosfera.Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Brechosfera.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly INotifier _notifier;

    protected ApiController(INotifier notifier)
    {
        _notifier = notifier;
    }

    protected bool IsValidOperation() => !_notifier.HasNotifications();

    protected ActionResult NotificationProblem()
    {
        foreach (var notification in _notifier.GetNotifications())
        {
            ModelState.AddModelError(
                notification.Code,
                notification.Message
            );
        }

        return ValidationProblem(ModelState);
    }
}
