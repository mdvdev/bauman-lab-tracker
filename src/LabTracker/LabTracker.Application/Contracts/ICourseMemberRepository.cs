using LabTracker.Domain.Entities;

namespace LabTracker.Application.Contracts;

public interface ICourseMemberRepository : ICrudRepository<CourseMember, CourseMemberKey>
{
    Task<List<CourseMember>> GetMembersByCourseIdAsync(Guid courseId);
    Task<List<CourseMember>> GetCoursesByMemberIdAsync(Guid memberId);
}