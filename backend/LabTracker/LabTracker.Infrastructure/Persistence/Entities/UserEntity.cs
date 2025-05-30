using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace LabTracker.Infrastructure.Persistence.Entities;

public class UserEntity : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Group { get; set; }
    public string Patronymic { get; set; }
    public string? TelegramUsername { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? PhotoUri { get; set; }

    public Users.Domain.User ToDomain(IEnumerable<string> roles)
    {
        return Users.Domain.User.Restore(
            id: Id,
            firstName: FirstName,
            lastName: LastName,
            group: Group,
            patronymic: Patronymic,
            roles: roles.Select(Enum.Parse<Role>).ToList(),
            email: Email,
            telegramUsername: TelegramUsername,
            createdAt: CreatedAt,
            photoUri: PhotoUri);
    }

    public static UserEntity FromDomain(Users.Domain.User user)
    {
        return new UserEntity
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Group = user.Group,
            Patronymic = user.Patronymic,
            TelegramUsername = user.TelegramUsername,
            CreatedAt = user.CreatedAt,
            Email = user.Email,
            UserName = user.Email,
            PhotoUri = user.PhotoUri
        };
    }
}