using LabTracker.Domain.Entities;

namespace LabTracker.Presentation.Dtos.Responses;

public class CourseMemberResponse
{
    public required DateTimeOffset AssignedAt { get; set; }
    public required Guid CourseId { get; set; }
    public required Guid MemberId { get; set; }

    public static CourseMemberResponse Create(CourseMember courseMember)
    {
        return new CourseMemberResponse
        {
            CourseId = courseMember.CourseId,
            MemberId = courseMember.MemberId,
            AssignedAt = courseMember.AssignedAt
        };
    }
}