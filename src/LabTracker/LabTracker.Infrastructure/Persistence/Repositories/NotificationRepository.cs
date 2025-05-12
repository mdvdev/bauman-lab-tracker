using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
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

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Notifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (entity is null)
            return null;

        var userRoles = entity.User is not null 
            ? await _context.UserRoles
                .Where(ur => ur.UserId == entity.User.Id)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync()
            : Enumerable.Empty<string>();

        return NotificationEntity.ToDomain(entity, userRoles);
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        var entities = await _context.Notifications
            .Include(n => n.User)
            .ToListAsync();

        var userIds = entities.Where(e => e.User is not null).Select(e => e.User.Id).Distinct();
        var userRoles = await _context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => new { ur.UserId, RoleName = r.Name })
            .ToListAsync();

        return entities.Select(entity => 
        {
            var roles = userRoles
                .Where(ur => ur.UserId == entity.User?.Id)
                .Select(ur => ur.RoleName);
            return NotificationEntity.ToDomain(entity, roles);
        });
    }

    public async Task<Guid> CreateAsync(Notification notification)
    {
        var entity = NotificationEntity.FromDomain(notification);
        await _context.Notifications.AddAsync(entity);
        await _context.SaveChangesAsync();
        return notification.Id;
    }

    public async Task UpdateAsync(Notification notification)
    {
        var entity = await _context.Notifications.FindAsync(notification.Id);
        if (entity is not null)
        {
            entity.Title = notification.Title;
            entity.Message = notification.Message;
            entity.Type = notification.Type.ToString();
            entity.IsRead = notification.IsRead;
            entity.ReadAt = notification.ReadAt;
            entity.RelatedEntityId = notification.RelatedEntityId;
            entity.RelatedEntityType = notification.RelatedEntityType;
            
            await _context.SaveChangesAsync();
        }
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
            query = query.Where(n => !n.IsRead) as IOrderedQueryable<NotificationEntity>;
        }

        var totalCount = await query.CountAsync();
        var unreadCount = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();

        var entities = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToListAsync();

        var items = entities.Select(e => NotificationEntity.ToDomain(e, userRoles));
        
        return (items, totalCount, unreadCount);
    }

    public async Task MarkAsReadAsync(Guid userId, IEnumerable<Guid> notificationIds)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
            .ToListAsync();

        await UpdateNotificationsAsReadAsync(notifications);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
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