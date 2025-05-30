using System.ComponentModel.DataAnnotations;
using LabTracker.Courses.Domain;

namespace LabTracker.Courses.Abstractions.Services.Dtos;

public record CreateCourseRequest(
    [Required] string Name,
    [Required] string Description,
    [Required] QueueMode QueueMode
);