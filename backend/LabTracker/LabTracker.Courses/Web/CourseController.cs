using Courses.Web.Dtos;
using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Domain;
using LabTracker.Courses.Abstractions.Services;
using LabTracker.Courses.Abstractions.Services.Dtos;
using LabTracker.Shared.Contracts;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Web;

[ApiController]
[Route("api/v1/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ICurrentUserService _currentUserService;

    public CourseController(
        ICourseService courseService,
        ICourseMemberService courseMemberService,
        ICurrentUserService currentUserService)
    {
        _courseService = courseService;
        _courseMemberService = courseMemberService;
        _currentUserService = currentUserService;
    }
    
    [HttpGet]
    [Authorize(nameof(Role.Administrator))]
    public async Task<IActionResult> GetCoursesAsync()
    {
        var courses = await _courseService.GetCoursesAsync();
        return Ok(courses.Select(CourseResponse.Create));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyCoursesAsync()
    {
        var user = _currentUserService.User;
        var courses = await _courseService.GetUserCoursesAsync(user.Id);
        return Ok(courses.Select(CourseResponse.Create));
    }

    [HttpPost]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> CreateCourseAsync(CreateCourseRequest request)
    {
        var course = await _courseService.CreateCourseAsync(request);
        var user = _currentUserService.User;

        await _courseMemberService.AddMemberAsync(new CourseMemberKey(course.Id, user.Id));

        return Ok(CourseResponse.Create(course));
    }

    [HttpGet("{courseId}")]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetCourseAsync(Guid courseId)
    {
        var course = await _courseService.GetCourseDetailsAsync(courseId);
        return course is null ? NotFound() : Ok(CourseResponse.Create(course));
    }

    [HttpPatch("{courseId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> PatchCourseAsync(Guid courseId, UpdateCourseRequest request)
    {
        var course = await _courseService.GetCourseDetailsAsync(courseId);
        if (course is null)
            return NotFound();

        course = await _courseService.UpdateCourseAsync(courseId, request);

        return Ok(CourseResponse.Create(course));
    }

    [HttpPatch("{courseId}/photo")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> PatchCoursePhotoAsync(Guid courseId, IFormFile file)
    {
        await _courseService.UpdateCoursePhotoAsync(courseId, file.OpenReadStream(), file.FileName);
        return Ok();
    }

    [HttpDelete("{courseId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> DeleteCourseAsync(Guid courseId)
    {
        await _courseService.DeleteCourseAsync(courseId);
        return Ok();
    }
}