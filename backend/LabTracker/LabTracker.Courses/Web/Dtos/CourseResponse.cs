using LabTracker.Courses.Domain;

namespace Courses.Web.Dtos;

public record CourseResponse(
    Guid Id,
    string Name,
    string Description,
    QueueMode QueueMode,
    DateTimeOffset CreatedAt,
    string? PhotoUri)
{
    public static CourseResponse Create(Course course) =>
        new(
            course.Id,
            course.Name,
            course.Description,
            course.QueueMode,
            course.CreatedAt,
            course.PhotoUri
        );
}