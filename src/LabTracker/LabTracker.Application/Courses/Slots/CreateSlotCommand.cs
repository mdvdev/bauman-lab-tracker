namespace LabTracker.Application.Courses.Slots;

public record CreateSlotCommand(
    Guid CourseId,
    Guid TeacherId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int MaxStudents
);