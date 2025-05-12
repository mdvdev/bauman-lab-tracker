using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;
public class CreateNotificationsBatchRequest
{
    public IEnumerable<NotificationItem> Notifications { get; set; }

    public class NotificationItem
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }
}