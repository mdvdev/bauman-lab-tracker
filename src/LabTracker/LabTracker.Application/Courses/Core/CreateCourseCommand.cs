using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses.Core;

public record CreateCourseCommand(
    CourseName Name,
    string Description,
    QueueMode QueueMode
);