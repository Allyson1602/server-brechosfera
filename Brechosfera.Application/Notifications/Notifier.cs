namespace Brechosfera.Application.Notifications;

public class Notifier : INotifier
{
    private readonly List<Notification> _notifications = [];

    public bool HasNotifications() => _notifications.Count > 0;

    public IReadOnlyCollection<Notification> GetNotifications() => _notifications.AsReadOnly();

    public void Add(string code, string message)
    {
        _notifications.Add(new Notification(code, message));
    }
}
