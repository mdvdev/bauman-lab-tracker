using System.ComponentModel.DataAnnotations;

namespace LabTracker.Application.Auth;

public record LoginCommand(
    string Email,
    string Password
);