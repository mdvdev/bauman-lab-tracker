namespace LabTracker.Presentation.Dtos;

public record MarkAsReadRequest(
    IEnumerable<Guid>? Ids,
    bool MarkAllAsRead = false);