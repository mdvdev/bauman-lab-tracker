using LabTracker.Notifications.Domain;

namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public class CreateNotificationsBatchRequest
{
    public IEnumerable<NotificationItem> Notifications { get; set; }

    public class NotificationItem
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
    }
}