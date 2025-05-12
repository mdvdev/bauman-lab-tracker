using System.ComponentModel.DataAnnotations;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class UserEntity : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }

    [MaxLength(50)] public string? TelegramUsername { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(500)] public string? PhotoUri { get; set; }

    public User ToDomain(IEnumerable<string> roles)
    {
        return new User
        {
            Id = Id,
            FirstName = new Name(FirstName),
            LastName = new Name(LastName),
            Patronymic = new Name(Patronymic),
            TelegramUsername = TelegramUsername,
            CreatedAt = CreatedAt,
            Email = Email,
            Roles = roles.Select(Enum.Parse<Role>).ToList(),
            PhotoUri = PhotoUri
        };
    }

    public static UserEntity FromDomain(User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Patronymic = user.Patronymic.Value,
            TelegramUsername = user.TelegramUsername,
            CreatedAt = user.CreatedAt,
            Email = user.Email,
            UserName = user.Email,
            PhotoUri = user.PhotoUri
        };
    }
}