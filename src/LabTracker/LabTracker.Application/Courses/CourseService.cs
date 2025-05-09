using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Courses;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly IUserRepository _userRepository;

    public CourseService(
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

    public async Task<Guid> CreateCourseAsync(CreateCourseCommand command)
    {
        var course = new Course
        {
            Name = command.Name,
            Description = command.Description,
            QueueMode = command.QueueMode
        };
        return await _courseRepository.CreateAsync(course);
    }

    public async Task<Course?> GetCourseDetailsAsync(Guid courseId)
    {
        return await _courseRepository.GetByIdAsync(courseId);
    }

    public async Task<CourseMember?> GetMemberDetailsAsync(Guid courseId, Guid memberId)
    {
        return await _courseMemberRepository.GetByIdAsync(new CourseMemberKey(courseId, memberId));
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

    public async Task UpdateCourseAsync(Guid courseId, UpdateCourseCommand command)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            throw new KeyNotFoundException($"Course with id {courseId} not found");

        if (command.Name is not null) course.Name = command.Name;
        if (command.Description is not null) course.Description = command.Description;
        if (command.QueueMode is not null) course.QueueMode = (QueueMode)command.QueueMode;

        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        await _courseRepository.DeleteAsync(courseId);
    }

    public async Task<CourseMemberKey> AddMemberAsync(Guid courseId, Guid memberId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        if (await _userRepository.GetByIdAsync(memberId) is null)
            throw new KeyNotFoundException($"User with id {memberId} not found.");

        return await _courseMemberRepository.CreateAsync(
            new CourseMember { CourseId = courseId, MemberId = memberId });
    }

    public async Task RemoveMemberAsync(Guid courseId, Guid memberId)
    {
        if (await _courseRepository.GetByIdAsync(courseId) is null)
            throw new KeyNotFoundException($"Course with '{courseId}' not found.");

        if (await _userRepository.GetByIdAsync(memberId) is null)
            throw new KeyNotFoundException($"User with id {memberId} not found.");

        await _courseMemberRepository.DeleteAsync(new CourseMemberKey(courseId, memberId));
    }
}