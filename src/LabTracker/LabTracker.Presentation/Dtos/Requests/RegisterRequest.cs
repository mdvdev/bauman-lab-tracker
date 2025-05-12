using System.ComponentModel.DataAnnotations;
using LabTracker.Application.Auth;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos.Requests;

public class RegisterRequest
{
    [Required] public required string Email { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string FirstName { get; set; }
    [Required] public required string LastName { get; set; }
    [Required] public required string Patronymic { get; set; }

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