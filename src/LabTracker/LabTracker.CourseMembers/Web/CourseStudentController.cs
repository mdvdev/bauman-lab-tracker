using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Domain;
using LabTracker.CourseMembers.Web.Dtos;
using LabTracker.Courses.Abstractions.Services;
using LabTracker.Courses.Domain;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Domain;
using LabTracker.User.Abstractions.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.CourseMembers.Web;

[ApiController]
[Route("api/v1/courses/{courseId}/students")]
public class CourseStudentController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly IUserRepository _userRepository;
    private readonly IOligarchStudentRepository _oligarchStudentRepository;

    public CourseStudentController(
        ICourseService courseService,
        ICourseMemberService courseMemberService,
        IUserRepository userRepository,
        IOligarchStudentRepository oligarchStudentRepository)
    {
        _courseService = courseService;
        _courseMemberService = courseMemberService;
        _userRepository = userRepository;
        _oligarchStudentRepository = oligarchStudentRepository;
    }

    [HttpGet]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetStudentsAsync(Guid courseId)
    {
        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        var students = await _courseMemberService
            .GetCourseMembersAsync(courseId, m => m.User.IsStudent);

        return Ok(students
            .Select(s => CourseMemberResponse.Create(s.CourseMember, course, s.User)));
    }

    [HttpPost("{studentId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> EnrollStudent(Guid courseId, Guid studentId)
    {
        var user = await _userRepository.GetByIdAsync(studentId);

        if (user is null)
            return NotFound($"User with ID '{studentId}' not found.");

        if (!user.IsStudent)
            return BadRequest($"User with ID '{studentId}' is not a student.");

        await _courseMemberService.AddMemberAsync(new CourseMemberKey(courseId, studentId));

        return Ok();
    }

    [HttpPatch("{studentId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> UpdateStudentStatus(Guid courseId, Guid studentId, [FromQuery] bool isOligarch)
    {
        var user = await _userRepository.GetByIdAsync(studentId);
        if (user is null)
            return NotFound($"User with ID '{studentId}' not found.");

        if (!user.IsStudent)
            return BadRequest($"User with ID '{studentId}' is not a student.");

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound($"Course with ID '{courseId}' not found.");

        var courseMember =
            await _courseMemberService.GetCourseMemberDetailsAsync(new CourseMemberKey(courseId, studentId));
        if (courseMember is null)
            return NotFound($"Course member with ID '{studentId}' not found.");

        if (course.QueueMode != QueueMode.Oligarchic)
            return BadRequest($"Course with ID '{courseId}' is not in oligarchic mode.");

        if (isOligarch)
            await _oligarchStudentRepository.CreateAsync(OligarchStudent.CreateNew(studentId, courseId));
        else
            await _oligarchStudentRepository.DeleteAsync(studentId, courseId);

        return Ok();
    }


    [HttpDelete("{studentId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> RemoveStudentFromCourse(Guid courseId, Guid studentId)
    {
        var user = await _userRepository.GetByIdAsync(studentId);

        if (user is null)
            return NotFound($"User with ID '{studentId}' not found.");

        if (!user.IsStudent)
            return BadRequest($"User with ID '{studentId}' is not a student.");

        await _courseMemberService.RemoveMemberAsync(new CourseMemberKey(courseId, studentId));

        return Ok();
    }
}