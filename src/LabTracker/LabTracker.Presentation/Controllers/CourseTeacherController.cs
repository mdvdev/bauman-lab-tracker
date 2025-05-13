using LabTracker.Application.Courses.Core;
using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos.Responses;
using LabTracker.Presentation.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId}/teachers")]
public class CourseTeacherController : ControllerBase
{
    private readonly ICourseMemberService _courseMemberService;
    private readonly ICourseService _courseService;
    private readonly ILogger<CourseTeacherController> _logger;

    public CourseTeacherController(
        ICourseMemberService courseMemberService,
        ICourseService courseService,
        ILogger<CourseTeacherController> logger)
    {
        _courseMemberService = courseMemberService;
        _courseService = courseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachers(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var courseTeachers = await _courseMemberService.GetCourseTeachersAsync(courseId);
        var response = new List<CourseMemberResponse>();
        foreach (var courseTeacher in courseTeachers)
        {
            var result = await CourseUtils
                .GetCourseAndTeacherAsync(courseId, courseTeacher.Id.MemberId, _courseService, _courseMemberService);
            if (result is null)
            {
                _logger.LogWarning(
                    "Course {CourseId} or Teacher {TeacherId} doesn't exist", courseId, courseTeacher.Id.MemberId);
                continue;
            }

            var (course, teacher) = result.Value;
            response.Add(CourseMemberResponse.Create(courseTeacher, course, teacher));
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize("TeacherOrAdmin")]
    public async Task<IActionResult> EnrollTeacher(Guid courseId, [FromQuery] Guid teacherId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!user.IsAdministrator &&
            !await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        await _courseMemberService.AddTeacherAsync(new CourseMemberKey(courseId, teacherId));
        return Ok();
    }
}