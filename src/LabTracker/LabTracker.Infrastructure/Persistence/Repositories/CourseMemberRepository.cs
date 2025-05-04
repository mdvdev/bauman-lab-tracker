using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class CourseMemberRepository : ICourseMemberRepository
{
    private readonly ApplicationDbContext _context;

    public CourseMemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CourseMember?> GetByIdAsync(CourseMemberKey key)
    {
        return await _context.CourseMembers.FindAsync(key.CourseId, key.MemberId);
    }

    public async Task<IEnumerable<CourseMember>> GetAllAsync()
    {
        return await _context.CourseMembers.ToListAsync();
    }

    public async Task<List<CourseMember>> GetMembersByCourseIdAsync(Guid courseId)
    {
        return await _context.CourseMembers.Where(cm => cm.CourseId == courseId).ToListAsync();
    }

    public async Task<List<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId)
    {
        return await _context.CourseMembers.Where(cm => cm.MemberId == memberId).ToListAsync();
    }

    public Task CreateAsync(CourseMember courseMember)
    {
        _context.CourseMembers.Add(courseMember);
        return _context.SaveChangesAsync();
    }

    public Task UpdateAsync(CourseMember courseMember)
    {
        var entry = _context.Entry(courseMember);
        if (entry.State == EntityState.Detached)
        {
            throw new InvalidOperationException("Entity is not being tracked.");
        }

        _context.CourseMembers.Update(courseMember);
        return _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CourseMemberKey key)
    {
        var member = await _context.CourseMembers.FindAsync(key.CourseId, key.MemberId);
        if (member is null)
        {
            throw new KeyNotFoundException(
                $"CourseMember with courseId '{key.CourseId}' and memberId '{key.MemberId}' not found.");
        }

        _context.CourseMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}