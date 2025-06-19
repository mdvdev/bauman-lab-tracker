namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public class MarkNotificationsReadRequest
{
    public IEnumerable<Guid>? NotificationIds { get; set; }
    public bool MarkAllAsRead { get; set; }
}