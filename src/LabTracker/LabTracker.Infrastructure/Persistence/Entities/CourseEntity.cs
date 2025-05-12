using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class CourseEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required QueueMode QueueMode { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string? PhotoUri { get; set; }

    public Course ToDomain()
    {
        return new Course
        {
            Id = Id,
            Name = new CourseName(Name),
            Description = Description,
            QueueMode = QueueMode,
            CreatedAt = CreatedAt,
            PhotoUri = PhotoUri
        };
    }

    public static CourseEntity FromDomain(Course course)
    {
        return new CourseEntity
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