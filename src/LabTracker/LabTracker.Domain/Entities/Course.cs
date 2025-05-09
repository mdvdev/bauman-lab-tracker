using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class Course
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required CourseName Name { get; set; }
    public required string Description { get; set; }
    public required QueueMode QueueMode { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? PhotoUri { get; set; }
}