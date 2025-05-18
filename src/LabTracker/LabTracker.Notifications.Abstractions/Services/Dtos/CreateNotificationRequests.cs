using LabTracker.Notifications.Domain;

namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}