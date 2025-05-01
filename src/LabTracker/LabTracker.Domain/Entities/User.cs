using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; }
    
    public Name FirstName { get; set; }
    
    public Name LastName { get; set; }
    
    public Name Patronymic { get; set; }

    public Telegram? Telegram { get; set; }
    
    public Role Role { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    [MaxLength(500)]
    public string? PhotoUri { get; set; }
}