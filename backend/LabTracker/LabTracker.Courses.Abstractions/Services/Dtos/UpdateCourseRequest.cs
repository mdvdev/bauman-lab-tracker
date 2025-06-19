using LabTracker.Courses.Domain;
using Shared;

namespace LabTracker.Courses.Abstractions.Services.Dtos;

[NotAllNull]
public record UpdateCourseRequest(
    string? Name,
    string? Description,
    QueueMode? QueueMode
);