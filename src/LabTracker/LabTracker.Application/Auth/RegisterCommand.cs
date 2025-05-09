using LabTracker.Domain.ValueObjects;

namespace LabTracker.Application.Auth;

public record RegisterCommand(
    string Email,
    string Password,
    Name FirstName,
    Name LastName,
    Name Patronymic
);