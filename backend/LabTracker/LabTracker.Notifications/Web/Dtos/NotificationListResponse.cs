namespace Notifications.Web.Dtos;

public class NotificationListResponse
{
    public IEnumerable<NotificationResponse> Items { get; set; }
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}