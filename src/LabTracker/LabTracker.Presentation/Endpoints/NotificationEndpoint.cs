using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos;
using System.Security.Claims;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var notificationGroup = endpoints.MapGroup("/notifications")
            .WithTags("Notifications")
            .RequireAuthorization()
            .WithOpenApi();

        notificationGroup.MapGet("/", async (
                [AsParameters] GetNotificationsQuery query,
                ClaimsPrincipal user,
                INotificationService notificationService) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var (items, totalCount, unreadCount) = await notificationService.GetUserNotificationsAsync(
                    userId,
                    query.Limit,
                    query.Offset,
                    query.UnreadOnly);

                return TypedResults.Ok(new NotificationResponse(
                    items.Select(ToDto),
                    totalCount,
                    unreadCount));
            })
            .Produces<NotificationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Получить свои уведомления")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Лимит количества уведомлений";
                operation.Parameters[1].Description = "Смещение (пагинация)";
                operation.Parameters[2].Description = "Только непрочитанные уведомления";
                return operation;
            });

        notificationGroup.MapPost("/", async (
            CreateNotificationRequest request,
            INotificationService notificationService) =>
        {
            if (!Enum.TryParse<NotificationType>(request.Type, out var notificationType))
            {
                return Results.BadRequest("Invalid notification type");
            }

            try
            {
                await notificationService.CreateNotificationAsync(
                    request.UserId,
                    request.Title,
                    request.Message,
                    notificationType,
                    request.RelatedEntityId,
                    request.RelatedEntityType);
                
                var notification = new Notification {
                    UserId = request.UserId,
                    Title = request.Title,
                    Message = request.Message,
                    Type = notificationType,
                    RelatedEntityId = request.RelatedEntityId,
                    RelatedEntityType = request.RelatedEntityType};

                return Results.Created($"/notifications/{notification.Id}", ToDto(notification));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole(nameof(Role.Administrator), nameof(Role.Teacher)))
        .Produces<NotificationDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .WithSummary("Создать новое уведомление")
        .WithOpenApi();

        notificationGroup.MapPost("/batch", async (
            CreateNotificationsBatchRequest request,
            INotificationService notificationService) =>
        {
            var notificationsToCreate = new List<(Guid, string, string, NotificationType, string?, string?)>();
            
            foreach (var notificationRequest in request.Notifications)
            {
                if (!Enum.TryParse<NotificationType>(notificationRequest.Type, out var notificationType))
                {
                    return Results.BadRequest($"Invalid notification type: {notificationRequest.Type}");
                }

                notificationsToCreate.Add((
                    notificationRequest.UserId,
                    notificationRequest.Title,
                    notificationRequest.Message,
                    notificationType,
                    notificationRequest.RelatedEntityId,
                    notificationRequest.RelatedEntityType));
            }

            try
            {
                await notificationService.CreateNotificationsBatchAsync(notificationsToCreate);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        })
        .RequireAuthorization(policy => policy.RequireRole(nameof(Role.Administrator), nameof(Role.Teacher)))
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .WithSummary("Создать несколько уведомлений")
        .WithOpenApi();

        notificationGroup.MapPost("/read", async (
                MarkAsReadRequest request,
                ClaimsPrincipal user,
                INotificationService notificationService) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                
                await notificationService.MarkNotificationsAsReadAsync(
                    userId,
                    request.Ids ?? Array.Empty<Guid>(),
                    request.MarkAllAsRead);

                return TypedResults.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithSummary("Пометить уведомления как прочитанные")
            .WithOpenApi();

        return endpoints;
    }
    private static NotificationDto ToDto(Notification notification) => new(
        notification.Id,
        notification.Title,
        notification.Message,
        notification.Type.ToString(),
        notification.IsRead,
        notification.CreatedAt,
        notification.ReadAt,
        notification.RelatedEntityId,
        notification.RelatedEntityType,
        UserDto.Create(notification.User, new List<Role>()));
}