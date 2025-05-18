using System.ComponentModel.DataAnnotations;

namespace LabTracker.Auth.Abstractions.Services.Dtos;

public class RegisterRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string Patronymic { get; set; }

    public RegisterRequest(string email, string password, string firstName, string lastName, string patronymic)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }
}