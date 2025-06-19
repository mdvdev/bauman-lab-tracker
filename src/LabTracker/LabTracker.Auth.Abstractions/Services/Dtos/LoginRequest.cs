using System.ComponentModel.DataAnnotations;

namespace LabTracker.Auth.Abstractions.Services.Dtos;

public class LoginRequest
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }

    public LoginRequest(string email, string password)
    {
        Email = email;
        Password = password;
    }
}