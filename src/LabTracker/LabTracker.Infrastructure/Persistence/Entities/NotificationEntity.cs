using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class NotificationEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public UserEntity User { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReadAt { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

    public static Notification ToDomain(NotificationEntity entity, IEnumerable<string> userRoles = null)
    {
        if (entity == null)
            return null;

        return new Notification
        {
            Id = entity.Id,
            UserId = entity.UserId,
            User = entity.User?.ToDomain(userRoles ?? Enumerable.Empty<string>()),
            Title = entity.Title,
            Message = entity.Message,
            Type = Enum.Parse<NotificationType>(entity.Type),
            IsRead = entity.IsRead,
            CreatedAt = entity.CreatedAt,
            ReadAt = entity.ReadAt,
            RelatedEntityId = entity.RelatedEntityId,
            RelatedEntityType = entity.RelatedEntityType
        };
    }

    public static NotificationEntity FromDomain(Notification domain)
    {
        if (domain == null)
            return null;

        return new NotificationEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            User = domain.User != null ? UserEntity.FromDomain(domain.User) : null,
            Title = domain.Title,
            Message = domain.Message,
            Type = domain.Type.ToString(),
            IsRead = domain.IsRead,
            CreatedAt = domain.CreatedAt,
            ReadAt = domain.ReadAt,
            RelatedEntityId = domain.RelatedEntityId,
            RelatedEntityType = domain.RelatedEntityType
        };
    }
}