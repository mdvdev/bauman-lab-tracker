using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Email Email { get; set; }
    
    [Required]
    public Name FirstName { get; set; }
    
    [Required]
    public Name LastName { get; set; }
    
    [Required]
    public Name Patronymic { get; set; }

    public Telegram? Telegram { get; set; }
    
    [Required]
    public Role Role { get; set; }
    
    [Required]
    public DateTimeOffset CreatedAt { get; set; }
    
    [MaxLength(500)]
    public string? Photo { get; set; }
}