namespace LabTracker.Presentation.Dtos;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? TelegramUsername { get; set; }
    public string? PhotoUri { get; set; }
}