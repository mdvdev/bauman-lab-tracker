using LabTracker.Notifications.Abstractions.Services.Dtos;
using LabTracker.Notifications.Domain;

namespace LabTracker.Notifications.Abstractions.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(CreateNotificationRequest request);

    Task CreateNotificationsBatchAsync(
        IEnumerable<(Guid userId, string title, string message, NotificationType type, string? relatedEntityId, string?
            relatedEntityType)> notifications);

    Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit = 20,
        int offset = 0,
        bool unreadOnly = false);

    Task MarkNotificationsAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds, bool markAllAsRead = false);
    Task<Notification> GetNotificationAsync(Guid notificationId);
}