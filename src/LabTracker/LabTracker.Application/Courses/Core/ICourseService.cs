using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Core;

public interface ICourseService
{
    Task<Guid> CreateCourseAsync(CreateCourseCommand command);
    Task<Course?> GetCourseDetailsAsync(Guid courseId);
    Task UpdateCourseAsync(Guid courseId, UpdateCourseCommand command);
    Task UpdateCoursePhotoAsync(Guid courseId, Stream stream, string fileName);
    Task DeleteCourseAsync(Guid courseId);
}