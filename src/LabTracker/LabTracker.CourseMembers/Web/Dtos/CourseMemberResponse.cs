using Courses.Web.Dtos;
using LabTracker.CourseMembers.Domain;
using LabTracker.Courses.Domain;
using Users.Web.Dtos;

namespace LabTracker.CourseMembers.Web.Dtos;

public record CourseMemberResponse(
    DateTimeOffset AssignedAt,
    CourseResponse Course,
    UserResponse User,
    int? Score,
    bool IsOligarch)
{
    public static CourseMemberResponse Create(CourseMember courseMember, Course course, Users.Domain.User user, bool isOligarch) =>
        new(
            AssignedAt: courseMember.AssignedAt,
            Course: CourseResponse.Create(course),
            User: UserResponse.Create(user),
            Score: courseMember.Score,
            IsOligarch: isOligarch
        );
}