namespace LabTracker.Presentation.Dtos.Requests;

public record GetNotificationsRequest(
    int Limit = 20,
    int Offset = 0,
    bool UnreadOnly = false);