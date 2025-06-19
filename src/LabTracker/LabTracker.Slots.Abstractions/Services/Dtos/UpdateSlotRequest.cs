using Shared;

namespace LabTracker.Slots.Abstractions.Services.Dtos;

[NotAllNull]
public record UpdateSlotRequest(
    DateTimeOffset? StartTime,
    DateTimeOffset? EndTime,
    int? MaxStudents
);