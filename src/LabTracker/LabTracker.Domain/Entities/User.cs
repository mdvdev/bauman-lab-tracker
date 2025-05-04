using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public Name FirstName { get; set; }
    public Name LastName { get; set; }
    public Name Patronymic { get; set; }

    [MaxLength(50)] public string? TelegramUsername { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(500)] public string? PhotoUri { get; set; }

    // Public for Identity API only. Otherwise, use below ctor.
    public User()
    {
    }
}