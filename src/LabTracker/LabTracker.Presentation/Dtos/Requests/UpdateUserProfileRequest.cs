using LabTracker.Application.Users;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class UpdateUserProfileRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? TelegramUsername { get; set; }

    public UpdateUserProfileCommand ToCommand()
    {
        return new UpdateUserProfileCommand(
            Email,
            FirstName is null ? null : new Name(FirstName),
            LastName is null ? null : new Name(LastName),
            Patronymic is null ? null : new Name(Patronymic),
            TelegramUsername
        );
    }
}