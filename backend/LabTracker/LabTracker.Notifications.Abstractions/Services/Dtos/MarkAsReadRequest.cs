namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public record MarkAsReadRequest(
    IEnumerable<Guid>? Ids,
    bool MarkAllAsRead = false
);