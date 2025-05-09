using LabTracker.Application.Courses;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class UpdateCourseRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public QueueMode? QueueMode { get; set; }

    public UpdateCourseCommand ToCommand()
    {
        return new UpdateCourseCommand(
            Name is null ? null : new CourseName(Name),
            Description,
            QueueMode
        );
    }
}