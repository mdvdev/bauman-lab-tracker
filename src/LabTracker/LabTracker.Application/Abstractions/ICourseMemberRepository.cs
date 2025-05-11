using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface ICourseMemberRepository : ICrudRepository<CourseMember, CourseMemberKey>
{
    Task<IEnumerable<CourseMember>> GetMembersByCourseIdAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId);
}