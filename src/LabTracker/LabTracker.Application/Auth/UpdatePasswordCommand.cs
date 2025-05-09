namespace LabTracker.Application.Auth;

public record UpdatePasswordCommand(
    string CurrentPassword,
    string NewPassword
);