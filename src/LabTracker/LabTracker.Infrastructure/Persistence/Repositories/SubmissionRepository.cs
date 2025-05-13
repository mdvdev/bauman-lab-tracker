using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Lab)
            .Include(s => s.Slot)
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (entity is null)
            return null;

        var userRoles = entity.Student is not null 
            ? await _context.UserRoles
                .Where(ur => ur.UserId == entity.Student.Id)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync()
            : Enumerable.Empty<string>();

        return SubmissionEntity.ToDomain(entity, userRoles);
    }

    public async Task<IEnumerable<Submission>> GetByCourseIdAsync(Guid courseId, Guid? studentId = null, string? status = null)
    {
        var query = _context.Submissions
            .Include(s => s.Student)
            .Include(s => s.Lab)
            .Include(s => s.Slot)
            .Include(s => s.Course)
            .Where(s => s.CourseId == courseId);

        if (studentId.HasValue)
        {
            query = query.Where(s => s.StudentId == studentId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var entities = await query.ToListAsync();

        var userIds = entities.Where(e => e.Student is not null).Select(e => e.Student.Id).Distinct();
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
                .Where(ur => ur.UserId == entity.Student?.Id)
                .Select(ur => ur.RoleName);
            return SubmissionEntity.ToDomain(entity, roles);
        });
    }

    public async Task<Guid> CreateAsync(Submission submission)
    {
        var entity = SubmissionEntity.FromDomain(submission);
        await _context.Submissions.AddAsync(entity);
        await _context.SaveChangesAsync();
        return submission.Id;
    }

    public async Task UpdateAsync(Submission submission)
    {
        var entity = await _context.Submissions.FindAsync(submission.Id);
        if (entity is not null)
        {
            entity.Status = submission.Status.ToString();
            entity.Score = submission.Score;
            entity.Comment = submission.Comment;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Submissions.FindAsync(id);
        if (entity is not null)
        {
            _context.Submissions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}