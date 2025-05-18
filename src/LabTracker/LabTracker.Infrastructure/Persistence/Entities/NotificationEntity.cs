using LabTracker.Notifications.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class NotificationEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public Notification ToDomain()
    {
        return Notification.Restore(
            id: Id,
            userId: UserId,
            title: Title,
            message: Message,
            type: Type,
            isRead: IsRead,
            createdAt: CreatedAt,
            readAt: ReadAt,
            relatedEntityId: RelatedEntityId,
            relatedEntityType: RelatedEntityType);
    }

    public static NotificationEntity FromDomain(Notification domain)
    {
        return new NotificationEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Title = domain.Title,
            Message = domain.Message,
            Type = domain.Type,
            IsRead = domain.IsRead,
            CreatedAt = domain.CreatedAt,
            ReadAt = domain.ReadAt,
            RelatedEntityId = domain.RelatedEntityId,
            RelatedEntityType = domain.RelatedEntityType
        };
    }
}