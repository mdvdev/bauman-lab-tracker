namespace LabTracker.Presentation.Dtos.Requests;

public record MarkAsReadRequest(
    IEnumerable<Guid>? Ids,
    bool MarkAllAsRead = false);