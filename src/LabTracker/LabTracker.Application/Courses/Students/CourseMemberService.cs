using LabTracker.Application.Abstractions;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

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

    public async Task<bool> IsCourseMemberAsync(CourseMemberKey key)
    {
        return await _courseMemberRepository.GetByIdAsync(key) != null;
    }

    public async Task<User?> GetCourseMemberDetailsAsync(CourseMemberKey key)
    {
        var member = await _courseMemberRepository.GetByIdAsync(key);
        if (member is null)
            return null;

        return await _userRepository.GetByIdAsync(member.Id.MemberId);
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
            var user = await _userRepository.GetByIdAsync(m.Id.MemberId);
            if (user is null)
                throw new KeyNotFoundException($"User with id {m.Id.MemberId} not found.");

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
            var user = await _userRepository.GetByIdAsync(m.Id.MemberId);
            if (user is null)
                throw new KeyNotFoundException($"User with id {m.Id.MemberId} not found.");

            if (user.IsTeacher)
                result.Add(m);
        }

        return result;
    }

    public async Task<CourseMember> AddStudentAsync(CourseMemberKey key)
    {
        var user = await _userRepository.GetByIdAsync(key.MemberId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {key.MemberId} not found.");

        if (!user.IsStudent)
            throw new InvalidOperationException($"User with id {key.MemberId} is not a student.");

        return await AddMemberAsync(key);
    }

    public async Task<CourseMember> AddTeacherAsync(CourseMemberKey key)
    {
        var user = await _userRepository.GetByIdAsync(key.MemberId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {key.MemberId} not found.");

        if (!user.IsTeacher)
            throw new InvalidOperationException($"User with id {key.MemberId} is not a teacher.");

        return await AddMemberAsync(key);
    }

    public async Task RemoveMemberAsync(CourseMemberKey key)
    {
        if (await _courseRepository.GetByIdAsync(key.CourseId) is null)
            throw new KeyNotFoundException($"Course with '{key.CourseId}' not found.");

        if (await _userRepository.GetByIdAsync(key.MemberId) is null)
            throw new KeyNotFoundException($"User with id {key.MemberId} not found.");

        await _courseMemberRepository.DeleteAsync(key);
    }

    private async Task<CourseMember> AddMemberAsync(CourseMemberKey key)
    {
        if (await _courseRepository.GetByIdAsync(key.CourseId) is null)
            throw new KeyNotFoundException($"Course with '{key.CourseId}' not found.");

        var user = await _userRepository.GetByIdAsync(key.MemberId);
        if (user is null)
            throw new KeyNotFoundException($"User with id {key.MemberId} not found.");

        return await _courseMemberRepository.CreateAsync(
            new CourseMember
            {
                Id = key,
                Score = user.IsStudent ? 0 : null
            });
    }
}