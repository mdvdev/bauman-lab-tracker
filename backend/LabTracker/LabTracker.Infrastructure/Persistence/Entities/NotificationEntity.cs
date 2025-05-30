using LabTracker.Notifications.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class NotificationEntity
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public UserEntity Sender { get; set; }
    public UserEntity Receiver { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    public Notification ToDomain()
    {
        return Notification.Restore(
            id: Id,
            senderId: SenderId,
            receiverId: ReceiverId,
            title: Title,
            message: Message,
            type: Type,
            isRead: IsRead,
            createdAt: CreatedAt,
            readAt: ReadAt);
    }

    public static NotificationEntity FromDomain(Notification domain)
    {
        return new NotificationEntity
        {
            Id = domain.Id,
            SenderId = domain.SenderId,
            ReceiverId = domain.ReceiverId,
            Title = domain.Title,
            Message = domain.Message,
            Type = domain.Type,
            IsRead = domain.IsRead,
            CreatedAt = domain.CreatedAt,
            ReadAt = domain.ReadAt,
        };
    }
}