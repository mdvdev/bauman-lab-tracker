using LabTracker.Application.Courses.Core;
using LabTracker.Application.Courses.Slots;
using LabTracker.Application.Courses.Students;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using LabTracker.Presentation.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/courses/{courseId}/slots")]
public class SlotController : ControllerBase
{
    private readonly ISlotService _slotService;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ICourseService _courseService;
    private readonly ILogger<SlotController> _logger;

    public SlotController(
        ISlotService slotService,
        ICourseMemberService courseMemberService,
        ICourseService courseService,
        ILogger<SlotController> logger)
    {
        _slotService = slotService;
        _courseMemberService = courseMemberService;
        _courseService = courseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCourseSlotsAsync(Guid courseId)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var slots = await _slotService.GetAllCourseSlotsAsync(courseId);
        var response = new List<SlotResponse>();

        foreach (var slot in slots)
        {
            var result = await CourseUtils
                .GetCourseAndTeacherAsync(slot.CourseId, slot.TeacherId, _courseService, _courseMemberService);
            if (result is null)
            {
                _logger.LogWarning("Slot {SlotId} is missing linked course or teacher", slot.Id);
                continue;
            }

            var (course, teacher) = result.Value;
            response.Add(SlotResponse.Create(slot, course, teacher));
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(nameof(Role.Teacher))]
    public async Task<IActionResult> CreateSlotAsync(Guid courseId, CreateSlotRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        if (!await _courseMemberService.IsCourseMemberAsync(new CourseMemberKey(courseId, user.Id)))
            return StatusCode(StatusCodes.Status403Forbidden);

        var slot = await _slotService.CreateSlotAsync(request.ToCommand(courseId, user.Id));

        var result = await CourseUtils
            .GetCourseAndTeacherAsync(slot.CourseId, slot.TeacherId, _courseService, _courseMemberService);
        if (result is null)
        {
            _logger.LogError("Slot {SlotId} was created but related Course or Teacher not found", slot.Id);
            return StatusCode(500, "Internal error: related data missing");
        }

        var (course, teacher) = result.Value;
        var response = SlotResponse.Create(slot, course, teacher);
        return Ok(response);
    }
}