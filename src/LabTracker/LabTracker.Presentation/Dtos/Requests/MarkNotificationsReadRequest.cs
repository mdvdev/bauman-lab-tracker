namespace LabTracker.Presentation.Dtos.Requests;

public class MarkNotificationsReadRequest
{
    public IEnumerable<Guid>? NotificationIds { get; set; }
    public bool MarkAllAsRead { get; set; }
}