using LabTracker.Shared.Contracts;
using LabTracker.User.Abstractions.Dtos;
using LabTracker.User.Abstractions.Services;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Web.Dtos;

namespace Users.Web;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UserController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
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
        var user = _currentUserService.User;
        return Ok(UserResponse.Create(user));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateCurrentUserAsync(UpdateUserProfileRequest request)
    {
        var user = _currentUserService.User;
        await _userService.UpdateUserProfileAsync(user.Id, request);
        return Ok();
    }

    [HttpPatch("me/photo")]
    public async Task<IActionResult> UpdateCurrentUserPhotoAsync(IFormFile file)
    {
        var user = _currentUserService.User;
        await _userService.UpdateProfilePhotoAsync(user.Id, file.OpenReadStream(), file.FileName);
        return Ok();
    }
}