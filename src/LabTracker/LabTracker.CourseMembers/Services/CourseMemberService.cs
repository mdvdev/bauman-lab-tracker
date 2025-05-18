using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Domain;
using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.User.Abstractions.Repositories;

namespace LabTracker.CourseMembers.Services;

public class CourseMemberService : ICourseMemberService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly IUserRepository _userRepository;

    public CourseMemberService(
        ICourseRepository courseRepository,
        ICourseMemberRepository courseMemberRepository,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _courseMemberRepository = courseMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> IsCourseMemberAsync(CourseMemberKey key)
    {
        return await _courseMemberRepository.GetByIdAsync(key) != null;
    }

    public async Task<CourseMemberInfo?> GetCourseMemberDetailsAsync(CourseMemberKey key)
    {
        var member = await _courseMemberRepository.GetByIdAsync(key);
        if (member is null)
            return null;

        var user = await _userRepository.GetByIdAsync(member.Id.UserId);
        if (user is null)
            throw new InvalidOperationException($"User with id {member.Id.UserId} not found.");
        
        return new CourseMemberInfo(member, user);
    }

    public async Task<IEnumerable<CourseMemberInfo>> GetCourseMembersAsync(
        Guid courseId,
        Func<CourseMemberInfo, bool>? predicate = null)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        var members = await _courseMemberRepository.GetMembersByCourseIdAsync(courseId);
        var result = new List<CourseMemberInfo>();

        foreach (var m in members)
        {
            var user = await _userRepository.GetByIdAsync(m.Id.UserId);
            if (user is null)
                throw new InvalidOperationException($"User with id {m.Id.UserId} not found.");

            var info = new CourseMemberInfo(m, user);

            if (predicate is null || predicate(info))
                result.Add(info);
        }

        return result;
    }

    public async Task RemoveMemberAsync(CourseMemberKey key)
    {
        if (await _courseRepository.GetByIdAsync(key.CourseId) is null)
            throw new KeyNotFoundException($"Course with '{key.CourseId}' not found.");

        if (await _userRepository.GetByIdAsync(key.UserId) is null)
            throw new KeyNotFoundException($"User with id {key.UserId} not found.");

        await _courseMemberRepository.DeleteAsync(key);
    }

    public async Task<CourseMember> AddMemberAsync(CourseMemberKey key)
    {
        if (await _courseRepository.GetByIdAsync(key.CourseId) is null)
            throw new KeyNotFoundException($"Course with '{key.CourseId}' not found.");

        var user = await _userRepository.GetByIdAsync(key.UserId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {key.UserId} not found.");

        return await _courseMemberRepository.CreateAsync(CourseMember.CreateNew(
            key: key,
            score: user.IsStudent ? 0 : null));
    }
}