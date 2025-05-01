namespace LabTracker.Api.Dtos;

public class LoginResponse
{
    public string AccessToken { get; set; }
    
    public UserDto User { get; set; }
}