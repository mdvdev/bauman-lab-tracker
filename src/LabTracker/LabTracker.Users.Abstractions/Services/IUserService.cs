using LabTracker.User.Abstractions.Dtos;

namespace LabTracker.User.Abstractions.Services;

public interface IUserService
{
    Task<IEnumerable<Users.Domain.User>> GetAllUsersAsync();
    Task<Users.Domain.User?> GetUserByIdAsync(Guid userId);
    Task UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request);
    Task UpdateProfilePhotoAsync(Guid userId, Stream stream, string fileName);
    Task<Dictionary<string, List<Users.Domain.User>>> GetGroupsWithStudentsAsync();
}