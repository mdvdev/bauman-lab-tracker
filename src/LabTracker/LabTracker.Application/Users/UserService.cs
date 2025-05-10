using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Users;

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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserProfileCommand command)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with id '{userId}' not found.");

        if (command.FirstName is not null) user.FirstName = command.FirstName;
        if (command.LastName is not null) user.LastName = command.LastName;
        if (command.Patronymic is not null) user.Patronymic = command.Patronymic;
        if (command.Email is not null) user.Email = command.Email;
        if (command.TelegramUsername is not null) user.TelegramUsername = command.TelegramUsername;

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

        user.PhotoUri = "/" + filePath;
        await _userRepository.UpdateAsync(user);
    }
}