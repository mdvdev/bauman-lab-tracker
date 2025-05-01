namespace LabTracker.Domain.Entities;

public class UserRefreshToken
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string RefreshToken { get; set; }
    
    public DateTimeOffset ExpiresAt { get; set; }
    
    public User User { get; set; }
}