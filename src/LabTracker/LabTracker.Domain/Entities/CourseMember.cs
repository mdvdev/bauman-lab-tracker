namespace LabTracker.Domain.Entities;

public class CourseMember
{
    public required Guid CourseId { get; init; }
    public required Guid MemberId { get; init; }
    public DateTimeOffset AssignedAt { get; init; } = DateTimeOffset.UtcNow;
    public required int? Score { get; init; }
}