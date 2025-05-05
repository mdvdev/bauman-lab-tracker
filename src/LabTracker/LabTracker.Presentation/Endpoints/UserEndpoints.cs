using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Presentation.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var users = endpoints
            .MapGroup("/users")
            .WithTags("Users");

        users.MapGet("/", async (UserManager<User> userManager) =>
        {
            var userList = await userManager.Users.ToListAsync();
            var result = new List<UserDto>();
            foreach (var user in userList)
            {
                var rolesString = await userManager.GetRolesAsync(user);
                var roles = rolesString.Select(Enum.Parse<Role>).ToList();
                result.Add(UserDto.Create(user, roles));
            }

            return Results.Ok(result);
        }).RequireAuthorization(nameof(Role.Administrator));


        users.MapGet("/me", async (
            HttpContext context, UserManager<User> userManager) =>
        {
            if (context.Items[ContextKeys.CurrentUser] is not User user)
                return Results.NotFound();

            var rolesString = await userManager.GetRolesAsync(user);
            var roles = rolesString.Select(Enum.Parse<Role>).ToList();

            return Results.Ok(UserDto.Create(user, roles));
        });


        users.MapPatch("/me", async (
            HttpContext context, UserManager<User> userManager, UpdateUserProfileDto dto) =>
        {
            if (dto.FirstName is null &&
                dto.LastName is null &&
                dto.Patronymic is null &&
                dto.TelegramUsername is null)
            {
                return Results.BadRequest("At least one field must be provided.");
            }

            if (context.Items[ContextKeys.CurrentUser] is not User user)
                return Results.NotFound();

            user.FirstName = dto.FirstName != null ? new Name(dto.FirstName) : user.FirstName;
            user.LastName = dto.LastName != null ? new Name(dto.LastName) : user.LastName;
            user.Patronymic = dto.Patronymic != null ? new Name(dto.Patronymic) : user.Patronymic;
            user.TelegramUsername = dto.TelegramUsername ?? user.TelegramUsername;

            await userManager.UpdateAsync(user);

            return Results.Ok();
        });


        users.MapPatch("/me/photo", async (
            HttpContext context, UserManager<User> userManager, IFileService fileService, IFormFile file) =>
        {
            try
            {
                const string saveDirectory = "StaticFiles/Images/ProfilePhotos";
                var filePath = await fileService.SaveImageAsync(file,
                    saveDirectory,
                    Path.GetFileNameWithoutExtension(file.FileName));

                if (context.Items[ContextKeys.CurrentUser] is not User user)
                    return Results.NotFound();

                user.PhotoUri = "/" + filePath;
                await userManager.UpdateAsync(user);

                return Results.Ok();
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(e.Message);
            }
        }).DisableAntiforgery();

        return endpoints;
    }
}