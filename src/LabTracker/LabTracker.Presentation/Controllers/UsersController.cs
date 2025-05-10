using LabTracker.Application.Users;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(nameof(Role.Administrator))]
    public async Task<IActionResult> GetUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Select(UserResponse.Create));
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();
        return Ok(UserResponse.Create(user));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateCurrentUserAsync(UpdateUserProfileRequest request)
    {
        if (request.FirstName is null &&
            request.LastName is null &&
            request.Patronymic is null &&
            request.TelegramUsername is null)
        {
            return BadRequest("At least one field must be provided.");
        }

        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        await _userService.UpdateUserProfileAsync(user.Id, request.ToCommand());

        return Ok();
    }

    [HttpPatch("me/photo")]
    public async Task<IActionResult> UpdateCurrentUserPhotoAsync(IFormFile file)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        await _userService.UpdateProfilePhotoAsync(user.Id, file.OpenReadStream(), file.FileName);

        return Ok();
    }
}