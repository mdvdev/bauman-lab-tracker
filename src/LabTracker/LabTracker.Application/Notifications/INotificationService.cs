using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Contracts;

public interface INotificationService
{
    Task CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? relatedEntityId = null,
        string? relatedEntityType = null);
    
    Task CreateNotificationsBatchAsync(IEnumerable<(Guid userId, string title, string message, NotificationType type, string? relatedEntityId, string? relatedEntityType)> notifications);
    Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit = 20,
        int offset = 0,
        bool unreadOnly = false);
    
    Task MarkNotificationsAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds, bool markAllAsRead = false);
}