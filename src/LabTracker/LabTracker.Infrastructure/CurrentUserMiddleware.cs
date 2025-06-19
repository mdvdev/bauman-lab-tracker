using System.Security.Claims;
using LabTracker.User.Abstractions.Repositories;
using Microsoft.AspNetCore.Http;
using Shared;

namespace LabTracker.Infrastructure;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst("sub") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    context.Items[ContextKeys.CurrentUser] = user;
                }
            }
        }

        await _next(context);
    }
}