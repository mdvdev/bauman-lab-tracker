using LabTracker.Notifications.Abstractions.Repositories;
using LabTracker.Notifications.Abstractions.Services;
using LabTracker.Notifications.Abstractions.Services.Dtos;
using LabTracker.Notifications.Domain;

namespace Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task CreateNotificationAsync(CreateNotificationRequest request)
    {
        var notification = Notification.CreateNew(
            userId: request.UserId,
            title: request.Title,
            message: request.Message,
            type: request.Type,
            relatedEntityId: request.RelatedEntityId,
            relatedEntityType: request.RelatedEntityType);

        await _notificationRepository.CreateAsync(notification);
    }

    public async Task CreateNotificationsBatchAsync(
        IEnumerable<(Guid userId, string title, string message, NotificationType type, string? relatedEntityId, string?
            relatedEntityType)> notifications)
    {
        var notificationEntities = notifications.Select(n => Notification.CreateNew(
            userId: n.userId,
            title: n.title,
            message: n.message,
            type: n.type,
            relatedEntityId: n.relatedEntityId,
            relatedEntityType: n.relatedEntityType));

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

    public async Task MarkNotificationsAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds,
        bool markAllAsRead = false)
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
 
    public async Task<Notification> GetNotificationAsync(Guid notificationId) 
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification is null)
            return null;
        
        return notification;
    }
}  