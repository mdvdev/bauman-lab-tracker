using LabTracker.Api.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = LabTracker.Api.Dtos.LoginRequest;
using RegisterRequest = LabTracker.Api.Dtos.RegisterRequest;

namespace LabTracker.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    [HttpGet("/register")]
    public IActionResult register(RegisterRequest registerRequest)
    {
        return Ok();
    }

    [HttpGet("/login")]
    public LoginResponse login(LoginRequest registerRequest)
    {
    }

    [HttpGet("/refresh")]
    public RefreshResponse refresh(RefreshRequest registerRequest)
    {
    }
}