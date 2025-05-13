using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Abstractions;

public interface ICourseMemberRepository : ICrudRepository<CourseMember, CourseMemberKey>
{
    Task<IEnumerable<CourseMember>> GetMembersByCourseIdAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId);
}