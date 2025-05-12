using LabTracker.Application.Auth;
using LabTracker.Domain.Entities;
using LabTracker.Presentation.Dtos.Requests;
using LabTracker.Presentation.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var user = await _authService.LoginAsync(request.ToCommand());
        return user == null ? BadRequest() : Ok(UserResponse.Create(user));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request.ToCommand());
        return user == null ? BadRequest() : Ok(UserResponse.Create(user));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authService.LogoutAsync();
        return Ok();
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePasswordAsync(UpdateUserPasswordRequest request)
    {
        if (HttpContext.Items[ContextKeys.CurrentUser] is not User user)
            return NotFound();

        await _authService.UpdatePasswordAsync(user.Id, request.ToCommand());

        return Ok();
    }
}