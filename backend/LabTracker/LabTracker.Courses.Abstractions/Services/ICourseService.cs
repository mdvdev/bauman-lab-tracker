using LabTracker.Courses.Abstractions.Services.Dtos;
using LabTracker.Courses.Domain;

namespace LabTracker.Courses.Abstractions.Services;

public interface ICourseService
{
    Task<Course> CreateCourseAsync(CreateCourseRequest command);
    Task<Course?> GetCourseDetailsAsync(Guid courseId);
    Task<IEnumerable<Course>> GetCoursesAsync();
    Task<IEnumerable<Course>> GetUserCoursesAsync(Guid userId);
    Task<Course> UpdateCourseAsync(Guid courseId, UpdateCourseRequest command);
    Task<Course> UpdateCoursePhotoAsync(Guid courseId, Stream stream, string fileName);
    Task DeleteCourseAsync(Guid courseId);
}