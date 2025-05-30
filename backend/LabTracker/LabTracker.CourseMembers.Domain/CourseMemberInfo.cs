using LabTracker.Users.Domain;

namespace LabTracker.CourseMembers.Domain;

public record CourseMemberInfo(CourseMember CourseMember, User User);