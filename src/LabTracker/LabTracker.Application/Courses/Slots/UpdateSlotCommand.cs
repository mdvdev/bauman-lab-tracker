namespace LabTracker.Application.Courses.Slots;

public record UpdateSlotCommand(
    Guid SlotId,
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    int? MaxStudents
);