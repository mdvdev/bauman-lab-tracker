using LabTracker.Application.Courses.Core;
using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId}/students")]
public class CourseStudentController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ILogger<CourseStudentController> _logger;

    public CourseStudentController(
        ICourseService courseService,
        ICourseMemberService courseMemberService,
        ILogger<CourseStudentController> logger)
    {
        _courseService = courseService;
        _courseMemberService = courseMemberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!user.IsAdministrator && !await _courseMemberService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        var students = await _courseMemberService.GetCourseStudentsAsync(courseId);
        var responses = new List<CourseMemberResponse>();
        foreach (var cm in students)
        {
            var localUser = await _courseMemberService.GetCourseMemberDetailsAsync(courseId, cm.MemberId);
            if (localUser == null)
            {
                _logger.LogWarning("User with ID {UserId} for course {CourseId} not found in database", cm.MemberId,
                    courseId);
                continue;
            }

            responses.Add(CourseMemberResponse.Create(cm, course, localUser));
        }

        return Ok(responses);
    }

    [HttpPost("{studentId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> EnrollStudentToCourse(Guid courseId, Guid studentId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (user.IsTeacher && !await _courseMemberService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        await _courseMemberService.AddStudentAsync(courseId, studentId);

        return Ok();
    }
}