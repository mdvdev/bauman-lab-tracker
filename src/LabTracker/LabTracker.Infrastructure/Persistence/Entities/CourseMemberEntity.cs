using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class CourseMemberEntity
{
    public required Guid CourseId { get; set; }
    public required Guid MemberId { get; set; }
    public required DateTimeOffset AssignedAt { get; set; }
    public required int? Score { get; set; }

    public CourseEntity Course { get; init; }
    public UserEntity User { get; init; }

    public CourseMember ToDomain()
    {
        return new CourseMember
        {
            Id = new CourseMemberKey(CourseId, MemberId),
            AssignedAt = AssignedAt,
            Score = Score
        };
    }

    public static CourseMemberEntity FromDomain(CourseMember courseMember)
    {
        return new CourseMemberEntity
        {
            CourseId = courseMember.Id.CourseId,
            MemberId = courseMember.Id.MemberId,
            AssignedAt = courseMember.AssignedAt,
            Score = courseMember.Score
        };
    }
}