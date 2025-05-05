using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Presentation.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints
            .MapGroup("/auth")
            .WithTags("Auth");

        auth.MapCustomizedIdentityApi<User>();

        auth.MapPost("/logout", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        });


        auth.MapPatch("/password", async (
            HttpContext context, UserManager<User> userManager, UpdateUserPasswordDto dto) =>
        {
            if (context.Items[ContextKeys.CurrentUser] is not User user)
                return Results.NotFound();

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            return !result.Succeeded ? Results.BadRequest() : Results.Ok();
        });

        return endpoints;
    }
}