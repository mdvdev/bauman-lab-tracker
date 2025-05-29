using LabTracker.Admin.Abstractions.Services;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabTracker.Admin.Web;

[ApiController]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("{userId}/roles")]
    [Authorize(nameof(Role.Administrator))]
    public async Task<IActionResult> AddRoleToUser(Guid userId, [FromQuery] Role role)
    {
        await _adminService.AddRoleToUserAsync(userId, role);
        return Ok();
    }

    [HttpDelete("{userId}/roles")]
    [Authorize(nameof(Role.Administrator))]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, [FromQuery] Role role)
    {
        await _adminService.RemoveRoleFromUserAsync(userId, role);
        return Ok();
    }
}