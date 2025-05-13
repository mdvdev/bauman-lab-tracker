namespace LabTracker.Domain.Entities;

public class Slot
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid CourseId { get; init; }
    public required Guid TeacherId { get; init; }
    public required DateTimeOffset StartTime { get; set; }
    public required DateTimeOffset EndTime { get; set; }
    public required int MaxStudents { get; set; }
}