using Labs.Web.Dtos;
using LabTracker.Labs.Abstractions.Services;
using LabTracker.Labs.Abstractions.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Labs.Web;

[ApiController]
[Route("api/v1/courses/{courseId}/labs")]
public class LabController : ControllerBase
{
    private readonly ILabService _labService;

    public LabController(ILabService labService)
    {
        _labService = labService;
    }

    [HttpGet]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetLabsAsync(Guid courseId)
    {
        var labs = await _labService.GetLabsByCourseIdAsync(courseId);
        var responses = labs.Select(LabResponse.Create).ToList();
        return Ok(responses);
    }

    [HttpGet("{labId}")]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetLabAsync(Guid courseId, Guid labId)
    {
        var lab = await _labService.GetLabByIdAsync(labId);
        if (lab is null || lab.CourseId != courseId)
            return NotFound();

        return Ok(LabResponse.Create(lab));
    }

    [HttpPost]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> CreateLabAsync(Guid courseId, CreateLabRequest request)
    {
        var lab = await _labService.CreateLabAsync(courseId, request);
        return Ok(LabResponse.Create(lab));
    }

    [HttpPatch("{labId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> UpdateLabAsync(Guid courseId, Guid labId, UpdateLabRequest request)
    {
        await _labService.UpdateLabAsync(labId, request);
        return Ok();
    }

    [HttpPatch("{labId}/description")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> UpdateLabDescriptionAsync(Guid courseId, Guid labId, IFormFile file)
    {
        await _labService.UpdateLabDescriptionAsync(labId, file.OpenReadStream(), file.FileName);
        return Ok();
    }

    [HttpDelete("{labId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> DeleteLabAsync(Guid courseId, Guid labId)
    {
        await _labService.DeleteLabAsync(labId);
        return Ok();
    }
}