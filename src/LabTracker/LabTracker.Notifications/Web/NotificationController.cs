using LabTracker.Notifications.Abstractions.Services;
using LabTracker.Notifications.Abstractions.Services.Dtos;
using LabTracker.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notifications.Web.Dtos;

namespace Notifications.Web;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationController(INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificationsAsync(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0,
        [FromQuery] bool unreadOnly = false)
    {
        var user = _currentUserService.User;

        var (items, totalCount, unreadCount) = await _notificationService
            .GetUserNotificationsAsync(user.Id, limit, offset, unreadOnly);

        var response = new NotificationListResponse
        {
            Items = items.Select(NotificationResponse.Create),
            TotalCount = totalCount,
            UnreadCount = unreadCount
        };

        return Ok(response);
    }

    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotificationAsync(Guid notificationId)
    {
        var notification = await _notificationService.GetNotificationAsync(notificationId);
        if (notification is null)
            return NotFound();

        return Ok(NotificationResponse.Create(notification));
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> CreateNotificationAsync(CreateNotificationRequest request)
    {
        var user = _currentUserService.User;
        var senderId = user.Id;
        await _notificationService.CreateNotificationAsync(senderId, request);
        return Ok();
    }

    [HttpPatch("read")]
    [Authorize]
    public async Task<IActionResult> MarkAsReadAsync(MarkNotificationsReadRequest request)
    {
        var user = _currentUserService.User;

        await _notificationService.MarkNotificationsAsReadAsync(
            user.Id,
            request.NotificationIds ?? [],
            request.MarkAllAsRead);

        return Ok();
    }
}