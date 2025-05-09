using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses;

public record UpdateCourseCommand(
    CourseName? Name,
    string? Description,
    QueueMode? QueueMode
);