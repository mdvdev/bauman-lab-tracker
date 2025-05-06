using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    public async Task CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? relatedEntityId = null,
        string? relatedEntityType = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };

        await _notificationRepository.CreateAsync(notification);
    }

    public async Task CreateNotificationsBatchAsync(
        IEnumerable<(Guid userId, string title, string message, NotificationType type, string? relatedEntityId, string? relatedEntityType)> notifications)
    {
        var notificationEntities = notifications.Select(n => new Notification
        {
            UserId = n.userId,
            Title = n.title,
            Message = n.message,
            Type = n.type,
            RelatedEntityId = n.relatedEntityId,
            RelatedEntityType = n.relatedEntityType
        });

        await _notificationRepository.CreateBatchAsync(notificationEntities);
    }

    public async Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit = 20,
        int offset = 0,
        bool unreadOnly = false)
    {
        return await _notificationRepository.GetUserNotificationsAsync(
            userId,
            limit,
            offset,
            unreadOnly);
    }

    public async Task MarkNotificationsAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds, bool markAllAsRead = false)
    {
        if (markAllAsRead)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }
        else
        {
            await _notificationRepository.MarkAsReadAsync(userId, notificationIds);
        }
    }
}