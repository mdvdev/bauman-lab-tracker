using System.ComponentModel.DataAnnotations;
using LabTracker.Application.Auth;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class RegisterRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string Patronymic { get; set; }

    public RegisterCommand ToCommand()
    {
        return new RegisterCommand(
            Email,
            Password,
            new Name(FirstName),
            new Name(LastName),
            new Name(Patronymic)
        );
    }
}