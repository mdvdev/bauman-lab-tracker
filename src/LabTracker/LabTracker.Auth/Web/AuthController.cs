using LabTracker.Auth.Abstractions.Services;
using LabTracker.Auth.Abstractions.Services.Dtos;
using LabTracker.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Web;
using Users.Web.Dtos;

namespace Auth.Web;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var user = await _authService.LoginAsync(request);
        return Ok(UserResponse.Create(user));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request);
        return Ok(UserResponse.Create(user));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authService.LogoutAsync();
        return Ok();
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordRequest request)
    {
        var user = _currentUserService.User;
        await _authService.UpdatePasswordAsync(user.Id, request);
        return Ok();
    }
}