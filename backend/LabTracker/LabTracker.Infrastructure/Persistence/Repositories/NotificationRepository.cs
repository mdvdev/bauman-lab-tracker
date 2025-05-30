using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Notifications.Abstractions.Repositories;
using LabTracker.Notifications.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Notifications.FindAsync(id);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        var entities = await _context.Notifications.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        if (await _context.Notifications.FindAsync(notification.Id) is null)
        {
            await _context.Notifications.AddAsync(NotificationEntity.FromDomain(notification));
            await _context.SaveChangesAsync();
        }

        return notification;
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        var entity = await _context.Notifications.FindAsync(notification.Id);

        if (entity is null) return await CreateAsync(notification);

        entity.Title = notification.Title;
        entity.Message = notification.Message;
        entity.Type = notification.Type;
        entity.IsRead = notification.IsRead;
        entity.ReadAt = notification.ReadAt;

        _context.Notifications.Update(entity);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Notifications.FindAsync(id);
        if (entity is not null)
        {
            _context.Notifications.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateBatchAsync(IEnumerable<Notification> notifications)
    {
        var entities = notifications.Select(NotificationEntity.FromDomain);
        await _context.Notifications.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    // TODO: Refactor.
    public async Task<(IEnumerable<Notification> Items, int TotalCount, int UnreadCount)> GetUserNotificationsAsync(
        Guid userId,
        int limit,
        int offset,
        bool unreadOnly)
    {
        var query = _context.Notifications
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.CreatedAt);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead) as IOrderedQueryable<NotificationEntity>;
        }

        var totalCount = await query.CountAsync();
        var unreadCount = await _context.Notifications
            .Where(n => n.ReceiverId == userId && !n.IsRead)
            .CountAsync();

        var entities = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var items = entities.Select(e => e.ToDomain());

        return (items, totalCount, unreadCount);
    }

    public async Task MarkAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds)
    {
        var notifications = await _context.Notifications
            .Where(n => n.SenderId == userId && notificationIds.Contains(n.Id))
            .ToListAsync();

        await UpdateNotificationsAsReadAsync(notifications);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.SenderId == userId && !n.IsRead)
            .ToListAsync();

        await UpdateNotificationsAsReadAsync(notifications);
    }

    private async Task UpdateNotificationsAsReadAsync(List<NotificationEntity> notifications)
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