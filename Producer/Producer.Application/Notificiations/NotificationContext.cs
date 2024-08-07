namespace Producer.Application.Notificiations;

public class NotificationContext
{
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public bool HasNotifications => _notifications.Any();

    public List<Notification> AddNotification(string key, string message)
    {
        _notifications.Add(new Notification(key, message));
        return _notifications;
    }

    public List<Notification> AddNotifications(IEnumerable<Notification> notifications)
    {
        _notifications.AddRange(notifications);
        return _notifications;
    }

    public List<Notification> AddNotifications(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
            AddNotification(error.ErrorCode, error.ErrorMessage);

        return _notifications;
    }
}
