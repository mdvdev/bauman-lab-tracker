namespace LabTracker.Presentation.Dtos;

public record NotificationResponse(
    IEnumerable<NotificationDto> Items,
    int TotalCount,
    int UnreadCount);