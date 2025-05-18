using LabTracker.CourseMembers.Domain;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class CourseMemberEntity
{
    public Guid CourseId { get; set; }
    public Guid MemberId { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
    public int? Score { get; set; }

    public CourseEntity Course { get; init; }
    public UserEntity User { get; init; }

    public CourseMember ToDomain(Users.Domain.User user)
    {
        return CourseMember.Restore(
            id: new CourseMemberKey(CourseId, MemberId),
            assignedAt: AssignedAt,
            score: Score
        );
    }

    public static CourseMemberEntity FromDomain(CourseMember courseMember)
    {
        return new CourseMemberEntity
        {
            CourseId = courseMember.Id.CourseId,
            MemberId = courseMember.Id.UserId,
            AssignedAt = courseMember.AssignedAt,
            Score = courseMember.Score,
        };
    }
}