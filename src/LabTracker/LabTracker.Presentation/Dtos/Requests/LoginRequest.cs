using System.ComponentModel.DataAnnotations;
using LabTracker.Application.Auth;

namespace LabTracker.Presentation.Dtos.Requests;

public class LoginRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }

    public LoginCommand ToCommand()
    {
        return new LoginCommand(Email, Password);
    }
}