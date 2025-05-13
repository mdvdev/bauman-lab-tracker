using LabTracker.Application.Contracts;
using LabTracker.Application.Submission;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId}/submissions")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly ILogger<SubmissionController> _logger;

    public SubmissionController(
        ISubmissionService submissionService,
        ILogger<SubmissionController> logger)
    {
        _submissionService = submissionService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSubmissionAsync(
        Guid courseId,
        [FromBody] CreateSubmissionRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return Unauthorized();

        try
        {
            var submission = await _submissionService.CreateSubmissionAsync(
                courseId,
                user.Id,
                request.LabId,
                request.SlotId);

            return CreatedAtAction(
                nameof(GetSubmissionAsync),
                new { courseId, submissionId = submission.Id },
                SubmissionResponse.Create(submission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission for user {UserId}", user.Id);
            return BadRequest(new { error = "Failed to register for submission", details = new[] { ex.Message } });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSubmissionsAsync(
        Guid courseId,
        [FromQuery] string? status = null)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return Unauthorized();

        try
        {
            var submissions = await _submissionService.GetSubmissionsAsync(
                courseId,
                user.Id,
                status);

            return Ok(submissions.Select(SubmissionResponse.Create));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submissions for user {UserId}", user.Id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{submissionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSubmissionAsync(
        Guid courseId,
        Guid submissionId)
    {
        try
        {
            await _submissionService.DeleteSubmissionAsync(courseId, submissionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting submission {SubmissionId}", submissionId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("{submissionId}/status")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> UpdateSubmissionStatusAsync(
        Guid courseId,
        Guid submissionId,
        [FromBody] UpdateSubmissionStatusRequest request)
    {
        try
        {
            var submission = await _submissionService.UpdateSubmissionStatusAsync(
                courseId,
                submissionId,
                request.Status,
                request.Score,
                request.Comment);

            return Ok(SubmissionResponse.Create(submission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for submission {SubmissionId}", submissionId);
            return BadRequest(new { error = "Failed to update submission status", details = new[] { ex.Message } });
        }
    }
}