using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }
    public string? TelegramUsername { get; set; }
    public List<string> Roles { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? PhotoUri { get; set; }

    public static UserDto Create(User user, List<Role> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Patronymic = user.Patronymic.Value,
            TelegramUsername = user.TelegramUsername,
            Roles = roles.Select(role => role.ToString()).ToList(),
            CreatedAt = user.CreatedAt,
            PhotoUri = user.PhotoUri
        };
    }
}