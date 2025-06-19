using Courses.Web.Dtos;
using LabTracker.CourseMembers.Domain;
using LabTracker.Courses.Domain;
using Users.Web;
using Users.Web.Dtos;

namespace LabTracker.CourseMembers.Web.Dtos;

public record CourseMemberResponse(
    DateTimeOffset AssignedAt,
    CourseResponse Course,
    UserResponse User,
    int? Score)
{
    public static CourseMemberResponse Create(CourseMember courseMember, Course course, Users.Domain.User user) =>
        new(
            AssignedAt: courseMember.AssignedAt,
            Course: CourseResponse.Create(course),
            User: UserResponse.Create(user),
            Score: courseMember.Score
        );
}