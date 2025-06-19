using LabTracker.Notifications.Abstractions.Services.Dtos;
using LabTracker.Notifications.Domain;

namespace LabTracker.Notifications.Abstractions.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(Guid senderId, CreateNotificationRequest request);

    Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit = 20,
        int offset = 0,
        bool unreadOnly = false);

    Task MarkNotificationsAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds, bool markAllAsRead = false);
    Task<Notification?> GetNotificationAsync(Guid notificationId);
}