using LabTracker.Application.Auth;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    public AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User?> RegisterAsync(RegisterCommand command)
    {
        var userEntity = new UserEntity
        {
            Email = command.Email,
            UserName = command.Email,
            FirstName = command.FirstName.Value,
            LastName = command.LastName.Value,
            Patronymic = command.Patronymic.Value,
        };

        var result = await _userManager.CreateAsync(userEntity, command.Password);
        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(userEntity, nameof(Role.Student));

        return userEntity.ToDomain(new List<string> { nameof(Role.Student) });
    }

    public async Task<User?> LoginAsync(LoginCommand command)
    {
        var userEntity = await _userManager.FindByEmailAsync(command.Email);
        if (userEntity is null)
            return null;

        var result = await _signInManager.PasswordSignInAsync(userEntity, command.Password, isPersistent: false,
            lockoutOnFailure: false);
        return !result.Succeeded ? null : userEntity.ToDomain(await _userManager.GetRolesAsync(userEntity));
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordCommand command)
    {
        var userEntity = await _userManager.FindByIdAsync(userId.ToString());
        if (userEntity is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        var result = await _userManager.ChangePasswordAsync(userEntity, command.CurrentPassword, command.NewPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Password change for {userId} failed.");
    }
}