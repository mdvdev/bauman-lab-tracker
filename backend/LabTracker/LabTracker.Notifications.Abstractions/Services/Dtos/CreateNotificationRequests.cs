using LabTracker.Notifications.Domain;

namespace LabTracker.Notifications.Abstractions.Services.Dtos;

public record CreateNotificationRequest(
    Guid ReceiverId,
    string Title,
    string Message,
    NotificationType Type
);