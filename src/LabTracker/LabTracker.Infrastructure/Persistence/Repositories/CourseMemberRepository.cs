using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
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
        var entity = await _context.CourseMembers.FindAsync(key.CourseId, key.MemberId);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<CourseMember>> GetAllAsync()
    {
        var entities = await _context.CourseMembers.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<CourseMember>> GetMembersByCourseIdAsync(Guid courseId)
    {
        var entities = await _context.CourseMembers
            .Where(cm => cm.CourseId == courseId)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<IEnumerable<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId)
    {
        var entities = await _context.CourseMembers
            .Where(cm => cm.MemberId == memberId)
            .ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<CourseMemberKey> CreateAsync(CourseMember courseMember)
    {
        if (await _context.CourseMembers
                .FindAsync(courseMember.CourseId, courseMember.MemberId) is null)
        {
            _context.CourseMembers.Add(CourseMemberEntity.FromDomain(courseMember));
            await _context.SaveChangesAsync();
        }

        return new CourseMemberKey(courseMember.CourseId, courseMember.MemberId);
    }

    public async Task UpdateAsync(CourseMember courseMember)
    {
        var entity = await _context.CourseMembers.FindAsync(courseMember.CourseId, courseMember.MemberId);
        if (entity is not null)
        {
            _context.CourseMembers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(CourseMemberKey key)
    {
        var member = await _context.CourseMembers.FindAsync(key.CourseId, key.MemberId);
        if (member is not null)
        {
            _context.CourseMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}