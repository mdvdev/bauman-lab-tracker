using System.ComponentModel.DataAnnotations;
using LabTracker.Application.Courses;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class CreateCourseRequest
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public QueueMode QueueMode { get; set; }

    public CreateCourseCommand ToCommand()
    {
        return new CreateCourseCommand(
            new CourseName(Name),
            Description,
            QueueMode
        );
    }
}