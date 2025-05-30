using System.ComponentModel.DataAnnotations;

namespace LabTracker.Auth.Abstractions.Services.Dtos;

public class UpdatePasswordRequest
{
    [Required] public required string CurrentPassword { get; set; }
    [Required] public required string NewPassword { get; set; }
}