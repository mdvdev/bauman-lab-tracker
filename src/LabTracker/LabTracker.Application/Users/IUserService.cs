using LabTracker.Domain.Entities;

namespace LabTracker.Application.Users;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task UpdateUserProfileAsync(Guid userId, UpdateUserProfileCommand command);
    Task UpdateProfilePhotoAsync(Guid userId, Stream stream, string fileName);
}