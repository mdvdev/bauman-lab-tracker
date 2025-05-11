using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Students;

public interface ICourseMemberService
{
    Task<bool> IsCourseMemberAsync(Guid courseId, Guid memberId);
    Task<User?> GetCourseMemberDetailsAsync(Guid courseId, Guid memberId);
    Task<IEnumerable<CourseMember>> GetMemberCoursesAsync(Guid memberId);
    Task<IEnumerable<CourseMember>> GetCourseStudentsAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetCourseTeachersAsync(Guid courseId);
    Task<CourseMemberKey> AddStudentAsync(Guid courseId, Guid userId);
    Task<CourseMemberKey> AddTeacherAsync(Guid courseId, Guid userId);
    Task RemoveMemberAsync(Guid courseId, Guid memberId);
}