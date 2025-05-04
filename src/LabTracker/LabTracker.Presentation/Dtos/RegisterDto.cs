using System.ComponentModel.DataAnnotations;

namespace LabTracker.Presentation.Dtos;

public class RegisterDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string Patronymic { get; set; }
}