using LabTracker.Application.Courses.Core;
using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ILogger<CourseController> _logger;

    public CourseController(
        ICourseService courseService,
        ICourseMemberService courseMemberService,
        ILogger<CourseController> logger)
    {
        _courseService = courseService;
        _courseMemberService = courseMemberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCoursesAsync()
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        var courseMembers = await _courseMemberService.GetMemberCoursesAsync(user.Id);
        var responses = new List<CourseMemberResponse>();
        foreach (var cm in courseMembers)
        {
            var localUser = await _courseMemberService.GetCourseMemberDetailsAsync(cm.Id);
            if (localUser == null)
            {
                _logger.LogWarning("User with ID {UserId} for course {CourseId} not found in database", cm.Id.MemberId,
                    cm.Id.CourseId);
                continue;
            }

            var course = await _courseService.GetCourseDetailsAsync(cm.Id.CourseId);
            if (course is null)
            {
                _logger.LogWarning("Course with ID {CourseId} not found in database", cm.Id.CourseId);
                continue;
            }

            responses.Add(CourseMemberResponse.Create(cm, course, localUser));
        }

        return Ok(responses);
    }

    [HttpPost]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> CreateCourseAsync(CreateCourseRequest request)
    {
        var course = await _courseService
            .CreateCourseAsync(request.ToCommand());

        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        await _courseMemberService.AddTeacherAsync(new CourseMemberKey(course.Id, user.Id));

        return Ok(CourseResponse.Create(course));
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourseAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        return course is null ? NotFound() : Ok(CourseResponse.Create(course));
    }

    [HttpPatch("{courseId}")]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> PatchCourseAsync(Guid courseId, UpdateCourseRequest request)
    {
        if (request.Name is null &&
            request.Description is null &&
            request.QueueMode is null)
        {
            return BadRequest("At least one field must be provided.");
        }

        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        course = await _courseService.UpdateCourseAsync(request.ToCommand(courseId));

        return Ok(CourseResponse.Create(course));
    }

    [HttpPatch("{courseId}/photo")]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> PatchCoursePhotoAsync(Guid courseId, IFormFile file)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        await _courseService.UpdateCoursePhotoAsync(courseId, file.OpenReadStream(), file.FileName);

        return Ok();
    }

    [HttpDelete("{courseId}")]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> DeleteCourseAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        await _courseService.DeleteCourseAsync(courseId);

        return Ok();
    }
}