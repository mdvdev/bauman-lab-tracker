using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Domain;
using LabTracker.CourseMembers.Web.Dtos;
using LabTracker.Courses.Abstractions.Services;
using LabTracker.User.Abstractions.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.CourseMembers.Web;

[ApiController]
[Route("api/v1/courses/{courseId}/teachers")]
public class CourseTeacherController : ControllerBase
{
    private readonly ICourseMemberService _courseMemberService;
    private readonly ICourseService _courseService;
    private readonly IUserRepository _userRepository;

    public CourseTeacherController(
        ICourseMemberService courseMemberService,
        ICourseService courseService,
        IUserRepository userRepository)
    {
        _courseMemberService = courseMemberService;
        _courseService = courseService;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetTeachers(Guid courseId)
    {
        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        var teachers = await _courseMemberService
            .GetCourseMembersAsync(courseId, m => m.User.IsTeacher);

        return Ok(teachers
            .Select(t => CourseMemberResponse.Create(t.CourseMember, course, t.User)));
    }

    [HttpPost]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> EnrollTeacher(Guid courseId, [FromQuery] Guid teacherId)
    {
        var user = await _userRepository.GetByIdAsync(teacherId);

        if (user is null)
            return NotFound($"User with ID '{teacherId}' not found.");

        if (!user.IsTeacher)
            return BadRequest($"User with ID '{teacherId}' is not a teacher.");

        await _courseMemberService.AddMemberAsync(new CourseMemberKey(courseId, teacherId));
        return Ok();
    }
}