namespace LabTracker.Presentation.Dtos;

public record GetNotificationsQuery(
    int Limit = 20,
    int Offset = 0,
    bool UnreadOnly = false);