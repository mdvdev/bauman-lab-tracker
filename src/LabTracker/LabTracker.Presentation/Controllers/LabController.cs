 using LabTracker.Application.Contracts.Labs;
 using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId}/labs")]
public class LabController : ControllerBase
{
    private readonly ILabService _labService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ILogger<LabController> _logger;

    public LabController(
        ILabService labService,
        ICourseMemberService courseMemberService,
        ILogger<LabController> logger)
    {
        _labService = labService;
        _courseMemberService = courseMemberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetLabsAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        

        try
        {
            var labs = await _labService.GetLabsByCourseIdAsync(courseId);
            var responses = labs.Select(LabResponse.Create).ToList();
            return Ok(responses);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course with ID {CourseId} not found", courseId);
            return NotFound();
        }
    }

    [HttpGet("{labId}")]
    public async Task<IActionResult> GetLabAsync(Guid courseId, Guid labId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        

        try
        {
            var lab = await _labService.GetLabByIdAsync(labId);
            if (lab is null || lab.CourseId != courseId)
                return NotFound();

            return Ok(LabResponse.Create(lab));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lab with ID {LabId} not found", labId);
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> CreateLabAsync(Guid courseId, CreateLabRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        

        try
        {
            var lab = await _labService.CreateLabAsync(
                courseId,
                request.Name,
                request.Description,
                request.Deadline,
                request.Score,
                request.ScoreAfterDeadline);

            return CreatedAtAction(
                nameof(GetLabAsync),
                new { courseId = lab.CourseId, labId = lab.Id },
                LabResponse.Create(lab));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course with ID {CourseId} not found", courseId);
            return NotFound();
        }
    }

    [HttpPatch("{labId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> UpdateLabAsync(Guid courseId, Guid labId, UpdateLabRequest request)
    {
        if (request.Name is null &&
            request.Description is null &&
            request.Deadline is null &&
            request.Score is null &&
            request.ScoreAfterDeadline is null)
        {
            return BadRequest("At least one field must be provided.");
        }

        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        

        try
        {
            await _labService.UpdateLabAsync(
                labId,
                request.Name ?? string.Empty,
                request.Description ?? string.Empty,
                request.Deadline ?? DateTimeOffset.UtcNow,
                request.Score ?? 0,
                request.ScoreAfterDeadline ?? 0);

            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lab with ID {LabId} not found", labId);
            return NotFound();
        }
    }

    [HttpDelete("{labId}")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> DeleteLabAsync(Guid courseId, Guid labId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (user.IsTeacher)
            return Forbid();

        try
        {
            var lab = await _labService.GetLabByIdAsync(labId);
            if (lab is null || lab.CourseId != courseId)
                return NotFound();

            await _labService.DeleteLabAsync(labId);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lab with ID {LabId} not found", labId);
            return NotFound();
        }
    }
}