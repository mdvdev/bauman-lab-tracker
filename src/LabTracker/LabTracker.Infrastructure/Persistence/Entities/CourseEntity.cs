using LabTracker.Courses.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class CourseEntity
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public QueueMode QueueMode { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public string? PhotoUri { get; set; }

    public Course ToDomain()
    {
        return Course.Restore(
            id: Id,
            name: Name,
            description: Description,
            queueMode: QueueMode,
            createdAt: CreatedAt,
            photoUri: PhotoUri);
    }

    public static CourseEntity FromDomain(Course course)
    {
        return new CourseEntity
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            QueueMode = course.QueueMode,
            CreatedAt = course.CreatedAt,
            PhotoUri = course.PhotoUri
        };
    }
}