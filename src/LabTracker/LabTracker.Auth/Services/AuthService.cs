using LabTracker.Auth.Abstractions.Services;
using LabTracker.Auth.Abstractions.Services.Dtos;
using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    public AuthService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var userEntity = new UserEntity
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Group = !string.IsNullOrEmpty(request.Group) ? request.Group.ToLowerInvariant() : null,
            Patronymic = request.Patronymic,
        };

        var result = await _userManager.CreateAsync(userEntity, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Failed to create user: {errors}");
        }

        await _userManager.AddToRoleAsync(userEntity, nameof(Role.Student));

        return userEntity.ToDomain(new List<string> { nameof(Role.Student) });
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var userEntity = await _userManager.FindByEmailAsync(request.Email);
        if (userEntity is null)
            throw new InvalidOperationException($"User with email '{request.Email}' does not exist");

        var result = await _signInManager.PasswordSignInAsync(userEntity, request.Password, isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            var reason = result.IsLockedOut ? "User is locked out." :
                result.IsNotAllowed ? "User is not allowed to log in." :
                result.RequiresTwoFactor ? "Two-factor authentication required." :
                "Invalid login attempt.";

            throw new InvalidOperationException($"Login failed: {reason}");
        }

        return userEntity.ToDomain(await _userManager.GetRolesAsync(userEntity));
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        var userEntity = await _userManager.FindByIdAsync(userId.ToString());
        if (userEntity is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        var result = await _userManager.ChangePasswordAsync(userEntity, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Password change for {userId} failed.");
    }
}