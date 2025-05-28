namespace LabTracker.Notifications.Domain;

public class Notification
{
    public Guid Id { get; }
    public Guid SenderId { get; }
    public Guid ReceiverId { get; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ReadAt { get; private set; }

    private Notification(
        Guid id,
        Guid senderId,
        Guid receiverId,
        string title,
        string message,
        NotificationType type,
        bool isRead,
        DateTimeOffset createdAt,
        DateTimeOffset? readAt)
    {
        if (senderId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(senderId));

        if (receiverId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(receiverId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));

        Id = id;
        SenderId = senderId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = isRead;
        CreatedAt = createdAt;
        ReadAt = readAt;
    }

    public static Notification CreateNew(
        Guid senderId,
        Guid receiverId,
        string title,
        string message,
        NotificationType type)
    {
        return new Notification(
            id: Guid.NewGuid(),
            senderId: senderId,
            receiverId: receiverId,
            title: title,
            message: message,
            type: type,
            isRead: false,
            createdAt: DateTimeOffset.UtcNow,
            readAt: null
        );
    }

    public static Notification Restore(
        Guid id,
        Guid senderId,
        Guid receiverId,
        string title,
        string message,
        NotificationType type,
        bool isRead,
        DateTimeOffset createdAt,
        DateTimeOffset? readAt)
    {
        return new Notification(
            id: id,
            senderId: senderId,
            receiverId: receiverId,
            title: title,
            message: message,
            type: type,
            isRead: isRead,
            createdAt: createdAt,
            readAt: readAt
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
}