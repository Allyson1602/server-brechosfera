using Brechosfera.Application.Notifications;
using Brechosfera.Domain.Abstractions;

namespace Brechosfera.Application.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly INotifier Notifier;

    protected BaseService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        UnitOfWork = unitOfWork;
        Notifier = notifier;
    }

    protected void Notify(string key, string message)
    {
        Notifier.Add(key, message);
    }

    protected async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await UnitOfWork.CommitAsync(cancellationToken);
    }
}
