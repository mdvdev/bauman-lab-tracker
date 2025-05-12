using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Users;

public record UpdateUserProfileCommand(
    string? Email,
    Name? FirstName,
    Name? LastName,
    Name? Patronymic,
    string? TelegramUsername
);