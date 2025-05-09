using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses;

public interface ICourseService
{
    Task<bool>IsCourseMemberAsync(Guid courseId, Guid memberId);
    Task<Guid> CreateCourseAsync(CreateCourseCommand command);
    Task<Course?> GetCourseDetailsAsync(Guid courseId);
    Task<CourseMember?> GetMemberDetailsAsync(Guid courseId, Guid memberId);
    Task<IEnumerable<CourseMember>> GetMemberCoursesAsync(Guid memberId);
    Task<IEnumerable<CourseMember>> GetCourseStudentsAsync(Guid courseId);
    Task<IEnumerable<CourseMember>> GetCourseTeachersAsync(Guid courseId);
    Task UpdateCourseAsync(Guid courseId, UpdateCourseCommand command);
    Task DeleteCourseAsync(Guid courseId);
    Task<CourseMemberKey> AddMemberAsync(Guid courseId, Guid memberId);
    Task RemoveMemberAsync(Guid courseId, Guid memberId);
}