using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Application.Courses;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseMemberRepository _courseMemberRepository;
    private readonly UserManager<User> _userManager;

    public CourseService(
        ICourseRepository courseRepository,
        ICourseMemberRepository courseMemberRepository,
        UserManager<User> userManager)
    {
        _courseRepository = courseRepository;
        _courseMemberRepository = courseMemberRepository;
        _userManager = userManager;
    }

    public async Task CreateCourseAsync(Course course)
    {
        await _courseRepository.CreateAsync(course);
    }

    public async Task<Course?> GetCourseDetailsAsync(Guid courseId)
    {
        return await _courseRepository.GetByIdAsync(courseId);
    }

    public async Task<CourseMember?> GetMemberDetailsAsync(Guid courseId, Guid memberId)
    {
        return await _courseMemberRepository.GetByIdAsync(new CourseMemberKey(courseId, memberId));
    }

    public async Task<List<CourseMember>> GetMemberCoursesAsync(Guid memberId)
    {
        return await _courseMemberRepository.GetCoursesByMemberIdAsync(memberId);
    }

    public async Task<List<CourseMember>> GetCourseStudentsAsync(Guid courseId)
    {
        var results = await GetMembersWithRoles(courseId);
        return results
            .Where(r => r.Roles.Contains(nameof(Role.Student)))
            .Select(r => r.Member)
            .ToList();
    }

    public async Task<List<CourseMember>> GetCourseTeachersAsync(Guid courseId)
    {
        var results = await GetMembersWithRoles(courseId);
        return results
            .Where(r => r.Roles.Contains(nameof(Role.Teacher)))
            .Select(r => r.Member)
            .ToList();
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        await _courseRepository.DeleteAsync(courseId);
    }

    public async Task AddMemberAsync(Guid courseId, Guid memberId)
    {
        var (course, member) = await GetCourseAndMemberAsync(courseId, memberId);
        await _courseMemberRepository.CreateAsync(new CourseMember(course, member));
    }

    public async Task RemoveMemberAsync(Guid courseId, Guid memberId)
    {
        await GetCourseAndMemberAsync(courseId, memberId); // Validates entities existence.
        await _courseMemberRepository.DeleteAsync(new CourseMemberKey(courseId, memberId));
    }

    private async Task<List<(CourseMember Member, IList<string> Roles)>> GetMembersWithRoles(Guid courseId)
    {
        var members = await _courseMemberRepository.GetMembersByCourseIdAsync(courseId);
        var memberTasks = members.Select(async m =>
        {
            var roles = await _userManager.GetRolesAsync(m.User);
            return (m, roles);
        });

        var membersWithRoles = await Task.WhenAll(memberTasks);

        return membersWithRoles.ToList();
    }

    private async Task<(Course, User)> GetCourseAndMemberAsync(Guid courseId, Guid memberId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null) throw new KeyNotFoundException($"Course with id '{courseId}' not found.");

        var member = await _userManager.FindByIdAsync(memberId.ToString());
        if (member is null) throw new KeyNotFoundException($"Member with id '{memberId}' not found.");

        return (course, member);
    }
}