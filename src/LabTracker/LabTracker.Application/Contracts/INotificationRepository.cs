using LabTracker.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabTracker.Application.Contracts;

public interface INotificationRepository: ICrudRepository<Notification, Guid>
{
    Task CreateAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<Notification> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task CreateBatchAsync(IEnumerable<Notification> notifications);
    Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId, 
        int limit, 
        int offset, 
        bool unreadOnly);
    
    Task MarkAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds);
    Task MarkAllAsReadAsync(Guid userId);
}