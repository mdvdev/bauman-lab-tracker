using LabTracker.Domain.Entities;

namespace LabTracker.Application.Auth;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterCommand command);
    Task<User?> LoginAsync(LoginCommand command);
    Task LogoutAsync();
    Task UpdatePasswordAsync(Guid userId, UpdatePasswordCommand command);
}