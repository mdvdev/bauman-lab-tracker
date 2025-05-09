using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses;

public record CreateCourseCommand(
    CourseName Name,
    string Description,
    QueueMode QueueMode
);