using LabTracker.Domain.Entities;

namespace LabTracker.Presentation.Dtos.Responses;

public class UserResponse
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Patronymic { get; set; }
    public string? TelegramUsername { get; set; }
    public required ICollection<string> Roles { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string? PhotoUri { get; set; }

    public static UserResponse Create(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            Patronymic = user.Patronymic.Value,
            TelegramUsername = user.TelegramUsername,
            Roles = user.Roles.Select(r => r.ToString()).ToList(),
            CreatedAt = user.CreatedAt,
            PhotoUri = user.PhotoUri
        };
    }
}