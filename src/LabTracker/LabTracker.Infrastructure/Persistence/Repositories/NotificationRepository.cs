using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Notification> CreateAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
        return notification;
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
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
            .ToListAsync();

        var now = DateTimeOffset.UtcNow;
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        var now = DateTimeOffset.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _context.SaveChangesAsync();
    }
}