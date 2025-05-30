using LabTracker.Admin.Abstractions.Services;
using LabTracker.User.Abstractions.Repositories;
using LabTracker.Users.Domain;

namespace LabTracker.Admin.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;

    public AdminService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddRoleToUserAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ArgumentException($"User with ID '{userId}' not found.");

        if (user.Roles.Contains(role))
            throw new ArgumentException($"User with ID '{userId}' already has role '{role}'.");

        await _userRepository.AddRoleToUserAsync(userId, role);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ArgumentException($"User with ID '{userId}' not found.");

        if (!user.Roles.Contains(role))
            throw new ArgumentException($"User with ID '{userId}' does not have role '{role}'.");

        await _userRepository.RemoveRoleFromUserAsync(userId, role);
    }
}