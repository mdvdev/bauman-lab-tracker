using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Presentation;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<UserEntity> userManager)
    {
        var userEntity = await userManager.GetUserAsync(context.User);
        if (userEntity != null)
        {
            var user = userEntity.ToDomain(await userManager.GetRolesAsync(userEntity));
            context.Items[ContextKeys.CurrentUser] = user;
        }

        await _next(context);
    }
}