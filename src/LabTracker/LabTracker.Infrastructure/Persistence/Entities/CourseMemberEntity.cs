using LabTracker.Domain.Entities;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class CourseMemberEntity
{
    public Guid CourseId { get; init; }
    public Guid MemberId { get; init; }
    public DateTimeOffset AssignedAt { get; init; }

    public CourseEntity Course { get; init; }
    public UserEntity User { get; init; }

    public CourseMember ToDomain()
    {
        return new CourseMember
        {
            CourseId = CourseId,
            MemberId = MemberId,
            AssignedAt = AssignedAt
        };
    }

    public static CourseMemberEntity FromDomain(CourseMember courseMember)
    {
        return new CourseMemberEntity
        {
            CourseId = courseMember.CourseId,
            MemberId = courseMember.MemberId,
            AssignedAt = courseMember.AssignedAt
        };
    }
}