using LabTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Presentation;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user != null)
        {
            context.Items[ContextKeys.CurrentUser] = user;
        }

        await _next(context);
    }
}