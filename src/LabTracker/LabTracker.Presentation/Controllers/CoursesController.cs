using LabTracker.Application.Courses;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<UsersController> _logger;

    public CoursesController(ICourseService courseService, ILogger<UsersController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        var courseMembers = await _courseService.GetMemberCoursesAsync(user.Id);
        return Ok(courseMembers);
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> PostAsync(CreateCourseRequest request)
    {
        var courseId = await _courseService
            .CreateCourseAsync(request.ToCommand());

        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (user.IsTeacher)
            await _courseService.AddMemberAsync(courseId, user.Id);

        return Ok();
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourseAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!user.IsAdministrator && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        return course is null ? NotFound() : Ok(CourseResponse.Create(course));
    }

    [HttpPatch("{courseId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
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

        if (user.IsTeacher && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        await _courseService.UpdateCourseAsync(courseId, request.ToCommand());

        return Ok();
    }

    [HttpPatch("{courseId}/photo")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> PatchCoursePhotoAsync(Guid courseId, IFormFile file)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        
        if (user.IsTeacher && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();
        
        await _courseService.UpdateCoursePhoto(courseId, file.OpenReadStream(), file.FileName);
        
        return Ok();
    }

    [HttpDelete("{courseId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> DeleteCourseAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (user.IsTeacher && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        await _courseService.DeleteCourseAsync(courseId);

        return Ok();
    }

    [HttpPost("{courseId}/students/{studentId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> PostAsync(Guid courseId, Guid studentId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (user.IsTeacher && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        await _courseService.AddMemberAsync(courseId, studentId);

        var courseMember = await _courseService.GetCourseMemberDetailsAsync(courseId, studentId);
        if (courseMember is null)
            return NotFound();

        return Ok();
    }

    [HttpGet("{courseId}/students/")]
    public async Task<IActionResult> GetStudentsAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!user.IsAdministrator && !await _courseService.IsCourseMemberAsync(courseId, user.Id))
            return Forbid();

        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        var students = await _courseService.GetCourseStudentsAsync(courseId);
        var responses = new List<CourseMemberResponse>();
        foreach (var cm in students)
        {
            var localUser = await _courseService.GetCourseMemberDetailsAsync(courseId, cm.MemberId);
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
}