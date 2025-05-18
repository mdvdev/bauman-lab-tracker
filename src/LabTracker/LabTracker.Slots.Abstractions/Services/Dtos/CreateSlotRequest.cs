namespace LabTracker.Slots.Abstractions.Services.Dtos;

public record CreateSlotRequest(
    Guid TeacherId,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    int MaxStudents
);