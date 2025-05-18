using LabTracker.Slots.Abstractions.Services;
using LabTracker.Slots.Abstractions.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slots.Web.Dtos;

namespace Slots.Web;

[ApiController]
[Route("api/v1/courses/{courseId}/slots")]
public class SlotController : ControllerBase
{
    private readonly ISlotService _slotService;

    public SlotController(ISlotService slotService)
    {
        _slotService = slotService;
    }

    [HttpGet]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetCourseSlotsAsync(Guid courseId)
    {
        var slots = await _slotService.GetCourseSlotsAsync(courseId);
        return Ok(slots.Select(SlotResponse.Create));
    }

    [HttpPost]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> CreateSlotAsync(Guid courseId, CreateSlotRequest request)
    {
        var slot = await _slotService.CreateSlotAsync(courseId, request);
        return Ok(SlotResponse.Create(slot));
    }

    [HttpGet("{slotId}")]
    [Authorize(Policy = "CourseMemberOnly")]
    public async Task<IActionResult> GetSlotAsync(Guid courseId, Guid slotId)
    {
        var slot = await _slotService.GetSlotAsync(slotId);
        return slot is null ? NotFound() : Ok(SlotResponse.Create(slot));
    }

    [HttpPatch("{slotId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> PatchSlotAsync(Guid courseId, Guid slotId, UpdateSlotRequest request)
    {
        var slot = await _slotService.GetSlotAsync(slotId);
        if (slot is null)
            return NotFound();

        await _slotService.UpdateSlotAsync(slotId, request);
        return Ok();
    }

    [HttpDelete("{slotId}")]
    [Authorize(Policy = "TeacherAndCourseMember")]
    public async Task<IActionResult> DeleteSlotAsync(Guid courseId, Guid slotId)
    {
        await _slotService.DeleteSlotAsync(slotId);
        return Ok();
    }
}