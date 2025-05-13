using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses.Core;

public record UpdateCourseCommand(
    Guid CourseId,
    CourseName? Name,
    string? Description,
    QueueMode? QueueMode
);