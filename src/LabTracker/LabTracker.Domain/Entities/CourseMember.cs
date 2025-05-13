using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class CourseMember
{
    public required CourseMemberKey Id { get; init; }
    public DateTimeOffset AssignedAt { get; init; } = DateTimeOffset.UtcNow;
    public required int? Score { get; init; }
}