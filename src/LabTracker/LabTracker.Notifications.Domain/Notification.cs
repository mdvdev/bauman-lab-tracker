namespace LabTracker.Notifications.Domain;

public class Notification
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ReadAt { get; private set; }
    public string? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }

    private Notification(
        Guid id,
        Guid userId,
        string title,
        string message,
        NotificationType type,
        bool isRead,
        DateTimeOffset createdAt,
        DateTimeOffset? readAt,
        string? relatedEntityId,
        string? relatedEntityType)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));

        Id = id;
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = isRead;
        CreatedAt = createdAt;
        ReadAt = readAt;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
    }

    public static Notification CreateNew(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? relatedEntityId = null,
        string? relatedEntityType = null)
    {
        return new Notification(
            id: Guid.NewGuid(),
            userId: userId,
            title: title,
            message: message,
            type: type,
            isRead: false,
            createdAt: DateTimeOffset.UtcNow,
            readAt: null,
            relatedEntityId: relatedEntityId,
            relatedEntityType: relatedEntityType
        );
    }

    public static Notification Restore(
        Guid id,
        Guid userId,
        string title,
        string message,
        NotificationType type,
        bool isRead,
        DateTimeOffset createdAt,
        DateTimeOffset? readAt,
        string? relatedEntityId,
        string? relatedEntityType)
    {
        return new Notification(
            id: id,
            userId: userId,
            title: title,
            message: message,
            type: type,
            isRead: isRead,
            createdAt: createdAt,
            readAt: readAt,
            relatedEntityId: relatedEntityId,
            relatedEntityType: relatedEntityType
        );
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTimeOffset.UtcNow;
        }
    }

    public void UpdateContent(string title, string message)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));

        Title = title;
        Message = message;
    }

    public void SetRelatedEntity(string? entityId, string? entityType)
    {
        RelatedEntityId = entityId;
        RelatedEntityType = entityType;
    }
}