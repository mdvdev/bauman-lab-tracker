namespace LabTracker.Presentation.Dtos;

public class UpdateUserPasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}