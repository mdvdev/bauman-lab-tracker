using LabTracker.Users.Domain;

namespace Users.Web.Dtos;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Group,
    string Patronymic,
    string? TelegramUsername,
    ICollection<string> Roles,
    DateTimeOffset CreatedAt,
    string? PhotoUri)
{
    public static UserResponse Create(User user) => new(
        Id: user.Id,
        Email: user.Email,
        FirstName: user.FirstName,
        LastName: user.LastName,
        Group: user.Group,
        Patronymic: user.Patronymic,
        TelegramUsername: user.TelegramUsername,
        Roles: user.Roles.Select(r => r.ToString()).ToList(),
        CreatedAt: user.CreatedAt,
        PhotoUri: user.PhotoUri
    );
}