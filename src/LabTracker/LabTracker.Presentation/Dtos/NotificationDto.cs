namespace LabTracker.Presentation.Dtos;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReadAt,
    string? RelatedEntityId,
    string? RelatedEntityType,
    UserDto Recipient);