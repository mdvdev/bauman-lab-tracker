using LabTracker.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LabTracker.Application.Users;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task UpdateUserProfileAsync(Guid userId, UpdateUserProfileCommand command);
    Task UpdateProfilePhotoAsync(Guid userId, IFormFile file);
}