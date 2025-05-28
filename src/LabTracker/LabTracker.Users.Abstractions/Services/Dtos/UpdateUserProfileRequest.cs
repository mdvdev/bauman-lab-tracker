using Shared;

namespace LabTracker.User.Abstractions.Services.Dtos;

[NotAllNull]
public record UpdateUserProfileRequest(
    string? Email,
    string? FirstName,
    string? LastName,
    string? Patronymic,
    string? TelegramUsername
);