using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Responses;

public class CourseResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required QueueMode QueueMode { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public string? PhotoUri { get; set; }

    public static CourseResponse Create(Course course)
    {
        return new CourseResponse
        {
            Id = course.Id,
            Name = course.Name.Value,
            Description = course.Description,
            QueueMode = course.QueueMode,
            CreatedAt = course.CreatedAt,
            PhotoUri = course.PhotoUri
        };
    }
}