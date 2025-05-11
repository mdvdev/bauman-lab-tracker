using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses.Core;

public record UpdateCourseCommand(
    CourseName? Name,
    string? Description,
    QueueMode? QueueMode
);