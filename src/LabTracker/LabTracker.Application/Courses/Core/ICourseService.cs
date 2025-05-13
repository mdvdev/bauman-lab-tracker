using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Core;

public interface ICourseService
{
    Task<Course> CreateCourseAsync(CreateCourseCommand command);
    Task<Course?> GetCourseDetailsAsync(Guid courseId);
    Task<Course> UpdateCourseAsync(UpdateCourseCommand command);
    Task<Course> UpdateCoursePhotoAsync(Guid courseId, Stream stream, string fileName);
    Task DeleteCourseAsync(Guid courseId);
}