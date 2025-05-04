using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses;

public interface ICourseService
{
    Task CreateCourseAsync(Course course);
    Task<Course?> GetCourseDetailsAsync(Guid courseId);
    Task<CourseMember?> GetMemberDetailsAsync(Guid courseId, Guid memberId);
    Task<List<CourseMember>> GetMemberCoursesAsync(Guid memberId);
    Task<List<CourseMember>> GetCourseStudentsAsync(Guid courseId);
    Task<List<CourseMember>> GetCourseTeachersAsync(Guid courseId);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Guid courseId);
    Task AddMemberAsync(Guid courseId, Guid memberId);
    Task RemoveMemberAsync(Guid courseId, Guid memberId);
}