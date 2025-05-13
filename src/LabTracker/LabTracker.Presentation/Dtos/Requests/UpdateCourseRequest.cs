using LabTracker.Application.Courses;
using LabTracker.Application.Courses.Core;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class UpdateCourseRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public QueueMode? QueueMode { get; set; }

    public UpdateCourseCommand ToCommand(Guid courseId)
    {
        return new UpdateCourseCommand(
            courseId,
            Name is null ? null : new CourseName(Name),
            Description,
            QueueMode
        );
    }
}