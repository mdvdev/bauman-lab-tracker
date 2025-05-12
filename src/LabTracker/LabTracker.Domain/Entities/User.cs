using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Domain.Entities;

public class User
{
    public required Guid Id { get; init; }
    
    public required Name FirstName { get; set; }
    
    public required Name LastName { get; set; }
    
    public required Name Patronymic { get; set; }
    
    public required string Email { get; set; }
    
    public required IEnumerable<Role> Roles;

    [MaxLength(50)]
    public required string? TelegramUsername { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    [MaxLength(500)]
    public required string? PhotoUri { get; set; }

    public bool IsTeacher => Roles.Contains(Role.Teacher);
    public bool IsStudent => Roles.Contains(Role.Student);
    public bool IsAdministrator => Roles.Contains(Role.Administrator);
}