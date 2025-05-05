namespace LabTracker.Presentation.Dtos;

public record CreateNotificationRequest(
    Guid UserId,
    string Title,
    string Message,
    string Type,
    string? RelatedEntityId = null,
    string? RelatedEntityType = null);

public record CreateNotificationsBatchRequest(
    IEnumerable<CreateNotificationRequest> Notifications);