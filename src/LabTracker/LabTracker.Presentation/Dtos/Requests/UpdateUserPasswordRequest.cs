using System.ComponentModel.DataAnnotations;
using LabTracker.Application.Auth;

namespace LabTracker.Presentation.Dtos.Requests;

public class UpdateUserPasswordRequest
{
    [Required] public required string CurrentPassword { get; set; }
    [Required] public required string NewPassword { get; set; }

    public UpdatePasswordCommand ToCommand()
    {
        return new UpdatePasswordCommand(
            CurrentPassword,
            NewPassword
        );
    }
}