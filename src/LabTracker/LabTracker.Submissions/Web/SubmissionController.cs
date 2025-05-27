using LabTracker.Shared.Contracts;
using LabTracker.Submissions.Abstractions.Services;
using LabTracker.Submissions.Abstractions.Services.Dtos;
using LabTracker.Submissions.Domain;
using LabTracker.Submissions.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Submissions.Web;

[ApiController]
[Route("api/v1/courses/{courseId}/submissions")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    private readonly ICurrentUserService _currentUserService;

    public SubmissionController(ISubmissionService submissionService, ICurrentUserService currentUserService)
    {
        _submissionService = submissionService;
        _currentUserService = currentUserService;
    }

    [HttpGet("{submissionId}")]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetSubmissionAsync(Guid courseId, Guid submissionId)
    {
        var submission = await _submissionService.GetSubmissionAsync(submissionId);
        if (submission is null)
            return NotFound();

        return Ok(SubmissionResponse.Create(submission));
    }

    [HttpPost]
    [Authorize(Policy = "StudentAndCourseMember")]
    public async Task<IActionResult> CreateSubmissionAsync(Guid courseId, CreateSubmissionRequest request)
    {
        var submission = await _submissionService.CreateSubmissionAsync(courseId, request);
        return Ok(SubmissionResponse.Create(submission));
    }

    [HttpGet]
    [Authorize(Policy = "StudentAndCourseMember")]
    public async Task<IActionResult> GetSubmissionsAsync(
        Guid courseId,
        [FromQuery] SubmissionStatus? status = null)
    {
        var submissions =
            await _submissionService.GetCourseSubmissionsAsync(courseId,
                status is not null ? s => s.Submission.SubmissionStatus == status : null);

        return Ok(submissions.Select(SubmissionResponse.Create));
    }

    [HttpGet("me")]
    [Authorize(Policy = "StudentAndCourseMember")]
    public async Task<IActionResult> GetMySubmissionsAsync(
        Guid courseId,
        [FromQuery] SubmissionStatus? status = null)
    {
        var user = _currentUserService.User;
        var submissions =
            await _submissionService.GetCourseSubmissionsAsync(courseId,
                s => s.Student.Id == user.Id);
        
        if (status is not null)
            submissions = submissions.Where(s => s.Submission.SubmissionStatus == status);

        return Ok(submissions.Select(SubmissionResponse.Create));
    }

    [HttpDelete("{submissionId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> DeleteSubmissionAsync(Guid courseId, Guid submissionId)
    {
        await _submissionService.DeleteSubmissionAsync(submissionId);
        return Ok();
    }

    [HttpPatch("{submissionId}/status")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> UpdateSubmissionStatusAsync(
        Guid courseId,
        Guid submissionId,
        UpdateSubmissionStatusRequest request)
    {
        var submission = await _submissionService.UpdateSubmissionStatusAsync(submissionId, request);
        return Ok(SubmissionResponse.Create(submission));
    }
}