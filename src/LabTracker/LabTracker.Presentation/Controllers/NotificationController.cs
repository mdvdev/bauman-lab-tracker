using LabTracker.Application.Contracts;
using LabTracker.Application.Notifications;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotificationsAsync(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0,
        [FromQuery] bool unreadOnly = false)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting notifications for user {UserId}", user.Id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> CreateNotificationAsync(CreateNotificationRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User currentUser)
            return NotFound();

        try
        {
            await _notificationService.CreateNotificationAsync(
                request.UserId,
                request.Title,
                request.Message,
                request.Type,
                request.RelatedEntityId,
                request.RelatedEntityType);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user {UserId}", request.UserId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("batch")]
    [Authorize(Policy = "TeacherOrAdmin")]
    public async Task<IActionResult> CreateNotificationsBatchAsync(CreateNotificationsBatchRequest request)
    {
        try
        {
            var notificationsData = request.Notifications.Select(n => 
                (n.UserId, n.Title, n.Message, n.Type, n.RelatedEntityId, n.RelatedEntityType));

            await _notificationService.CreateNotificationsBatchAsync(notificationsData);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating batch notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPatch("read")]
    [Authorize]
    public async Task<IActionResult> MarkAsReadAsync(MarkNotificationsReadRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        try
        {
            await _notificationService.MarkNotificationsAsReadAsync(
                user.Id,
                request.NotificationIds ?? Enumerable.Empty<Guid>(),
                request.MarkAllAsRead);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notifications as read for user {UserId}", user.Id);
            return StatusCode(500, "Internal server error");
        }
    }
}