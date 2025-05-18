using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.CourseMembers.Domain;
using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.User.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class CourseMemberRepository : ICourseMemberRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;

    public CourseMemberRepository(ApplicationDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<CourseMember?> GetByIdAsync(CourseMemberKey labId)
    {
        var entity = await _context.CourseMembers
            .FirstOrDefaultAsync(cm => cm.CourseId == labId.CourseId && cm.MemberId == labId.UserId);

        if (entity is null) return null;

        var user = await _userRepository.GetByIdAsync(entity.MemberId);
        return user is null ? null : entity.ToDomain(user);
    }

    public async Task<IEnumerable<CourseMember>> GetAllAsync()
    {
        var entities = await _context.CourseMembers.ToListAsync();
        return await ToDomainWithUsersAsync(entities);
    }

    public async Task<IEnumerable<CourseMember>> GetMembersByCourseIdAsync(Guid courseId)
    {
        var entities = await _context.CourseMembers
            .Where(cm => cm.CourseId == courseId)
            .ToListAsync();

        return await ToDomainWithUsersAsync(entities);
    }

    public async Task<IEnumerable<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId)
    {
        var entities = await _context.CourseMembers
            .Where(cm => cm.MemberId == memberId)
            .ToListAsync();

        return await ToDomainWithUsersAsync(entities);
    }

    public async Task<CourseMember> CreateAsync(CourseMember courseMember)
    {
        var exists = await _context.CourseMembers
            .AnyAsync(cm => cm.CourseId == courseMember.Id.CourseId && cm.MemberId == courseMember.Id.UserId);

        if (!exists)
        {
            _context.CourseMembers.Add(CourseMemberEntity.FromDomain(courseMember));
            await _context.SaveChangesAsync();
        }

        return courseMember;
    }

    public async Task<CourseMember> UpdateAsync(CourseMember courseMember)
    {
        var entity = await _context.CourseMembers
            .FirstOrDefaultAsync(cm =>
                cm.CourseId == courseMember.Id.CourseId && cm.MemberId == courseMember.Id.UserId);

        if (entity is null)
            return await CreateAsync(courseMember);

        entity.AssignedAt = courseMember.AssignedAt;
        entity.Score = courseMember.Score;

        _context.CourseMembers.Update(entity);
        await _context.SaveChangesAsync();

        return courseMember;
    }

    public async Task DeleteAsync(CourseMemberKey key)
    {
        var entity = await _context.CourseMembers
            .FirstOrDefaultAsync(cm => cm.CourseId == key.CourseId && cm.MemberId == key.UserId);

        if (entity is not null)
        {
            _context.CourseMembers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<List<CourseMember>> ToDomainWithUsersAsync(List<CourseMemberEntity> entities)
    {
        var result = new List<CourseMember>();

        foreach (var entity in entities)
        {
            var user = await _userRepository.GetByIdAsync(entity.MemberId);
            if (user != null)
            {
                result.Add(entity.ToDomain(user));
            }
        }

        return result;
    }
}