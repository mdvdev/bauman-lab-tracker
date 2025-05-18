using System.ComponentModel.DataAnnotations;
using LabTracker.Courses.Domain;

namespace LabTracker.Courses.Abstractions.Services.Dtos;

public record CreateCourseRequest(
    [property: Required] string Name,
    [property: Required] string Description,
    [property: Required] QueueMode QueueMode
);