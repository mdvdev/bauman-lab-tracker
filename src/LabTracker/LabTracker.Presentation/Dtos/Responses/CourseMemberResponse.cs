using LabTracker.Domain.Entities;

namespace LabTracker.Presentation.Dtos.Responses;

public class CourseMemberResponse
{
    public required DateTimeOffset AssignedAt { get; set; }
    public required CourseResponse Course { get; set; }
    public required UserResponse User { get; set; }
    public required int? Score { get; set; }

    public static CourseMemberResponse Create(CourseMember courseMember, Course course, User user)
    {
        return new CourseMemberResponse
        {
            Course = CourseResponse.Create(course),
            User = UserResponse.Create(user),
            AssignedAt = courseMember.AssignedAt,
            Score = courseMember.Score
        };
    }
}