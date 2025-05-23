using System.Text.RegularExpressions;

namespace LabTracker.Users.Domain;

public class User
{
    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Group  { get; private set; }
    public string Patronymic { get; private set; }
    public string Email { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public string? TelegramUsername { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public string? PhotoUri { get; private set; }

    private readonly List<Role> _roles;

    private static readonly Regex ValidNameRegex = new(@"^\p{L}+$", RegexOptions.Compiled);

    private User(
        Guid id,
        string firstName,
        string lastName,
        string group,
        string patronymic,
        string email,
        IReadOnlyCollection<Role> roles,
        DateTimeOffset createdAt,
        string? telegramUsername,
        string? photoUri)
    {
        if (string.IsNullOrWhiteSpace(firstName) || !IsValidName(firstName))
            throw new ArgumentException($"FirstName '{firstName}' is invalid.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName) || !IsValidName(lastName))
            throw new ArgumentException($"LastName '{lastName}' is invalid.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(patronymic) || !IsValidName(patronymic))
            throw new ArgumentException($"Patronymic '{patronymic}' is invalid.", nameof(patronymic));

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Group = group;
        Patronymic = patronymic;
        Email = email;
        _roles = roles.ToList();
        CreatedAt = createdAt;
        TelegramUsername = telegramUsername;
        PhotoUri = photoUri;
    }

    public static User CreateNew(
        string firstName,
        string lastName,
        string group,
        string patronymic,
        string email,
        IReadOnlyCollection<Role> roles,
        string? telegramUsername = null,
        string? photoUri = null)
    {
        return new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            group,
            patronymic,
            email,
            roles,
            DateTimeOffset.UtcNow,
            telegramUsername,
            photoUri);
    }

    public static User Restore(
        Guid id,
        string firstName,
        string lastName,
        string group,
        string patronymic,
        string email,
        IReadOnlyCollection<Role> roles,
        DateTimeOffset createdAt,
        string? telegramUsername = null,
        string? photoUri = null)
    {
        return new User(
            id,
            firstName,
            lastName,
            group,
            patronymic,
            email,
            roles,
            createdAt,
            telegramUsername,
            photoUri);
    }

    public void UpdateProfile(
        string? firstName = null,
        string? lastName = null,
        string? group = null,
        string? patronymic = null,
        string? email = null,
        string? telegramUsername = null,
        string? photoUri = null)
    {
        if (firstName is not null)
        {
            if (!IsValidName(firstName))
                throw new ArgumentException($"FirstName '{firstName}' is invalid.", nameof(firstName));
            FirstName = firstName;
        }

        if (lastName is not null)
        {
            if (!IsValidName(lastName))
                throw new ArgumentException($"LastName '{lastName}' is invalid.", nameof(lastName));
            LastName = lastName;
        }

        if (group is not null)
        {
            Group = group;
        }

        if (patronymic is not null)
        {
            if (!IsValidName(patronymic))
                throw new ArgumentException($"Patronymic '{patronymic}' is invalid.", nameof(patronymic));
            Patronymic = patronymic;
        }

        if (email is not null)
            Email = email;

        if (telegramUsername is not null)
            TelegramUsername = telegramUsername;

        if (photoUri is not null)
            PhotoUri = photoUri;
    }

    public bool IsTeacher => Roles.Contains(Role.Teacher);
    public bool IsStudent => Roles.Contains(Role.Student);
    public bool IsAdministrator => Roles.Contains(Role.Administrator);

    private static bool IsValidName(string value)
    {
        return ValidNameRegex.IsMatch(value);
    }
}