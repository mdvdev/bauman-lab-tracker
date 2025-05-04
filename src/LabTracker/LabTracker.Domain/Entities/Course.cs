using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Name Name { get; set; }
    public string Description { get; set; }
    public QueueMode QueueMode { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? PhotoUri { get; set; }

    private Course()
    {
    }

    public Course(Name name, string description, QueueMode queueMode)
    {
        Name = name;
        Description = description;
        QueueMode = queueMode;
    }
}