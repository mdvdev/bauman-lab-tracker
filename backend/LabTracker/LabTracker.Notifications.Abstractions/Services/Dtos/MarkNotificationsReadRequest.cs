namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public record MarkNotificationsReadRequest(
    IEnumerable<Guid>? NotificationIds,
    bool MarkAllAsRead
);