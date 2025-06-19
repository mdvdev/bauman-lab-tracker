using LabTracker.Notifications.Domain;
using LabTracker.Shared.Contracts;

namespace LabTracker.Notifications.Abstractions.Repositories;

public interface INotificationRepository : ICrudRepository<Notification, Guid>
{
    public Task CreateBatchAsync(IEnumerable<Notification> notifications);

    public Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit,
        int offset,
        bool unreadOnly);

    public Task MarkAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds);
    public Task MarkAllAsReadAsync(Guid userId);
}