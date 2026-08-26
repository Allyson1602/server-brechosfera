namespace Brechosfera.Application.Notifications;

public interface INotifier
{
    bool HasNotifications();
    IReadOnlyCollection<Notification> GetNotifications();
    void Add(string code, string message);
}
