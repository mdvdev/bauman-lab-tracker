using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses.Students;

public interface ICourseMemberService
{
    Task<bool> IsCourseMemberAsync(CourseMemberKey key);
    Task<User?> GetCourseMemberDetailsAsync(CourseMemberKey key);
    Task<IEnumerable<CourseMember>> GetMemberCoursesAsync(Guid memberId);
    Task<IEnumerable<CourseMember>> GetCourseStudentsAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetCourseTeachersAsync(Guid courseId);
    Task<CourseMember> AddStudentAsync(CourseMemberKey key);
    Task<CourseMember> AddTeacherAsync(CourseMemberKey key);
    Task RemoveMemberAsync(CourseMemberKey key);
}