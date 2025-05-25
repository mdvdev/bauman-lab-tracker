using LabTracker.CourseMembers.Domain;
using LabTracker.Shared.Contracts;

namespace LabTracker.CourseMembers.Abstractions.Repositories;

public interface ICourseMemberRepository : ICrudRepository<CourseMember, CourseMemberKey>
{
    Task<IEnumerable<CourseMember>> GetMembersByCourseIdAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetStudentsByCourseIdAsync(Guid courseId);
}