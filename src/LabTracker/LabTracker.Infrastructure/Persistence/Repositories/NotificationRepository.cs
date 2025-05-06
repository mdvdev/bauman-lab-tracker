using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        var notificationToUpdate = await _context.Notifications.FindAsync(notification.Id);
        if (notificationToUpdate is null)
        {
            throw new KeyNotFoundException($"Notification with {notification.Id} id not found");
        }
        notificationToUpdate.Title = notification.Title;
        notificationToUpdate.Message = notification.Message;
        notificationToUpdate.Type = notification.Type;
        
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        var notifications = await _context.Notifications.ToListAsync();
        if (notifications is null)
        {
            throw new KeyNotFoundException("There are no notifications");
        }
        return notifications;
    }

    public async Task<Notification> GetByIdAsync(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification is null)
        {
            throw new KeyNotFoundException($"Notification with id {id} not found");
        }
        return notification;
    }

    public async Task DeleteAsync(Guid id)
    {
        var notificationToDelete = _context.Notifications.Find(id);
        if (notificationToDelete is null)
        {
            throw new KeyNotFoundException($"Notification with id {id} not found");
        }
        _context.Notifications.Remove(notificationToDelete);
        _context.SaveChangesAsync();
    }

    public async Task CreateBatchAsync(IEnumerable<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId, 
        int limit, 
        int offset, 
        bool unreadOnly)
    {
        var query = _context.Notifications
            .Include(n => n.User) 
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead) as IOrderedQueryable<Notification>;
        }

        var totalCount = await query.CountAsync();
        var unreadCount = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();

        var items = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount, unreadCount);
    }

    public async Task MarkAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds)
    {
        var notifications = await GetNotificationsToMarkAsync(
            userId, 
            n => notificationIds.Contains(n.Id));
    
        await UpdateNotificationsAsReadAsync(notifications);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await GetNotificationsToMarkAsync(
            userId, 
            n => !n.IsRead);
    
        await UpdateNotificationsAsReadAsync(notifications);
    }

    private async Task<List<Notification>> GetNotificationsToMarkAsync(
        Guid userId, 
        Expression<Func<Notification, bool>> filter)
    {
        return await _context.Notifications
                   .Where(n => n.UserId == userId)
                   .Where(filter)
                   .ToListAsync() 
               ?? throw new KeyNotFoundException("Notifications not found");
    }

    private async Task UpdateNotificationsAsReadAsync(List<Notification> notifications)
    {
        var now = DateTimeOffset.UtcNow;
        notifications.ForEach(n => 
        {
            n.IsRead = true;
            n.ReadAt = now;
        });
    
        await _context.SaveChangesAsync();
    }
}