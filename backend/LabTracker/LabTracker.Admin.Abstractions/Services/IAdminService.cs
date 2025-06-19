using LabTracker.Users.Domain;

namespace LabTracker.Admin.Abstractions.Services;

public interface IAdminService
{
    Task AddRoleToUserAsync(Guid userId, Role role);
    Task RemoveRoleFromUserAsync(Guid userId, Role role);
}