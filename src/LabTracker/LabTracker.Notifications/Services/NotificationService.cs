using LabTracker.Notifications.Abstractions.Repositories;
using LabTracker.Notifications.Abstractions.Services;
using LabTracker.Notifications.Abstractions.Services.Dtos;
using LabTracker.Notifications.Domain;
using LabTracker.User.Abstractions.Services;

namespace Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserService _userService;

    public NotificationService(INotificationRepository notificationRepository, IUserService userService)
    {
        _notificationRepository = notificationRepository;
        _userService = userService;
    }

    public async Task CreateNotificationAsync(Guid senderId, CreateNotificationRequest request)
    {
        if (await _userService.GetUserByIdAsync(senderId) is null)
            throw new ArgumentException($"Sender with id {senderId} not found.");

        if (await _userService.GetUserByIdAsync(request.ReceiverId) is null)
            throw new ArgumentException($"Receiver with id {request.ReceiverId} not found.");

        if (senderId == request.ReceiverId)
            throw new ArgumentException("Sender and receiver cannot be the same.");

        var notification = Notification.CreateNew(
            senderId: senderId,
            receiverId: request.ReceiverId,
            title: request.Title,
            message: request.Message,
            type: request.Type);

        await _notificationRepository.CreateAsync(notification);
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

    public async Task<Notification?> GetNotificationAsync(Guid notificationId)
    {
        return await _notificationRepository.GetByIdAsync(notificationId);
    }
}