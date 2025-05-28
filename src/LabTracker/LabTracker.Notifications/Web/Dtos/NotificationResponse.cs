using LabTracker.Notifications.Domain;

namespace Notifications.Web.Dtos;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public static NotificationResponse Create(Notification notification) => new()
    {
        Id = notification.Id,
        SenderId = notification.SenderId,
        ReceiverId = notification.ReceiverId,
        Title = notification.Title,
        Message = notification.Message,
        Type = notification.Type,
        IsRead = notification.IsRead,
        CreatedAt = notification.CreatedAt,
        ReadAt = notification.ReadAt,
    };
}