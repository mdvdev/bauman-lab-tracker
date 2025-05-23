using LabTracker.Shared.Contracts;
using LabTracker.User.Abstractions.Dtos;
using LabTracker.User.Abstractions.Repositories;
using LabTracker.User.Abstractions.Services;
using LabTracker.Users.Domain;

namespace Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;

    private const string SaveDirectory = "StaticFiles/Images/ProfilePhotos";

    public UserService(IUserRepository userRepository, IFileService fileService)
    {
        _userRepository = userRepository;
        _fileService = fileService;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id '{userId}' not found.");

        user.UpdateProfile(
            request.FirstName, request.LastName, request.Patronymic, request.Email, request.TelegramUsername);

        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateProfilePhotoAsync(Guid userId, Stream stream, string fileName)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id '{userId}' not found.");

        var filePath = await _fileService.SaveFileAsync(
            stream,
            SaveDirectory,
            fileName
        );

        if (user.PhotoUri is not null)
            _fileService.DeleteFile(user.PhotoUri.TrimStart('/'));

        user.UpdateProfile(photoUri: "/" + filePath);

        await _userRepository.UpdateAsync(user);
    }
    public async Task<Dictionary<string, List<User>>> GetGroupsWithStudentsAsync()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var students = allUsers.Where(u => !string.IsNullOrEmpty(u.Group));
    
        return students
            .GroupBy(u => u.Group!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.ToList(),
                StringComparer.OrdinalIgnoreCase);
    }
}