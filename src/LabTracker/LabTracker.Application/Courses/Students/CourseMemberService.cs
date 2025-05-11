using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Courses.Students;

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

    public async Task<bool> IsCourseMemberAsync(Guid courseId, Guid memberId)
    {
        return await _courseMemberRepository.GetByIdAsync(new CourseMemberKey(courseId, memberId)) != null;
    }

    public async Task<User?> GetCourseMemberDetailsAsync(Guid courseId, Guid memberId)
    {
        var member = await _courseMemberRepository.GetByIdAsync(new CourseMemberKey(courseId, memberId));
        if (member is null)
            return null;

        return await _userRepository.GetByIdAsync(member.MemberId);
    }

    public async Task<IEnumerable<CourseMember>> GetMemberCoursesAsync(Guid memberId)
    {
        if (await _userRepository.GetByIdAsync(memberId) is null)
            throw new KeyNotFoundException($"Member with '{memberId}' not found.");
        return await _courseMemberRepository.GetCoursesByMemberIdAsync(memberId);
    }

    public async Task<IEnumerable<CourseMember>> GetCourseStudentsAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        var members = await _courseMemberRepository.GetMembersByCourseIdAsync(courseId);
        var result = new List<CourseMember>();

        foreach (var m in members)
        {
            var user = await _userRepository.GetByIdAsync(m.MemberId);
            if (user is null)
                throw new KeyNotFoundException($"User with id {m.MemberId} not found.");

            if (user.IsStudent)
                result.Add(m);
        }

        return result;
    }

    public async Task<IEnumerable<CourseMember>> GetCourseTeachersAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        var members = await _courseMemberRepository.GetMembersByCourseIdAsync(courseId);
        var result = new List<CourseMember>();

        foreach (var m in members)
        {
            var user = await _userRepository.GetByIdAsync(m.MemberId);
            if (user is null)
                throw new KeyNotFoundException($"User with id {m.MemberId} not found.");

            if (user.IsTeacher)
                result.Add(m);
        }

        return result;
    }

    public async Task<CourseMemberKey> AddStudentAsync(Guid courseId, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        if (!user.IsStudent)
            throw new InvalidOperationException($"User with id {userId} is not a student.");

        return await AddMemberAsync(courseId, userId);
    }

    public async Task<CourseMemberKey> AddTeacherAsync(Guid courseId, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");
        
        if (!user.IsTeacher)
            throw new InvalidOperationException($"User with id {userId} is not a teacher.");
        
        return await AddMemberAsync(courseId, userId);
    }

    public async Task RemoveMemberAsync(Guid courseId, Guid memberId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        if (await _userRepository.GetByIdAsync(memberId) is null)
            throw new KeyNotFoundException($"User with id {memberId} not found.");

        await _courseMemberRepository.DeleteAsync(new CourseMemberKey(courseId, memberId));
    }

    private async Task<CourseMemberKey> AddMemberAsync(Guid courseId, Guid userId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        return await _courseMemberRepository.CreateAsync(
            new CourseMember
            {
                CourseId = courseId,
                MemberId = userId,
                Score = user.IsStudent ? 0 : null
            });
    }
}