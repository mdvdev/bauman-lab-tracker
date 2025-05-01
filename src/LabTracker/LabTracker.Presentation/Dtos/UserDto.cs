namespace LabTracker.Api.Dtos;

public class UserDto
{
    public string Id { get; set; }
    
    public string Email { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Patronymic { get; set; }
    
    public string Telegram { get; set; }
    
    public string Role { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string PhotoUri { get; set; }
}