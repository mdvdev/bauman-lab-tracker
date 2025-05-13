using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public User Student { get; set; }
    public Guid LabId { get; set; }
    public Lab Lab { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid SlotId { get; set; }
    public Slot Slot { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
}