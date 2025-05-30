using LabTracker.CourseMembers.Domain;
using LabTracker.Users.Domain;

namespace LabTracker.CourseMembers.Abstractions.Services;

public interface ICourseMemberService
{
    Task<bool> IsCourseMemberAsync(CourseMemberKey key);
    Task<CourseMemberInfo?> GetCourseMemberDetailsAsync(CourseMemberKey key);

    Task<IEnumerable<CourseMemberInfo>> GetCourseMembersAsync(Guid courseId,
        Func<CourseMemberInfo, bool>? predicate = null);

    Task<CourseMember> AddMemberAsync(CourseMemberKey key);
    Task RemoveMemberAsync(CourseMemberKey key);
    
    Task AddScoreToStudent(CourseMemberKey key, int score);
}