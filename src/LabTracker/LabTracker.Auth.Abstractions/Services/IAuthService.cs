using LabTracker.Auth.Abstractions.Services.Dtos;
using LabTracker.Users.Domain;

namespace LabTracker.Auth.Abstractions.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<User> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request);
}