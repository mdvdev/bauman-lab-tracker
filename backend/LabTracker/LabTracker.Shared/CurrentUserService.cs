using LabTracker.Shared.Contracts;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Http;

namespace Shared;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User User => _httpContextAccessor.HttpContext?.Items[ContextKeys.CurrentUser] as User
                        ?? throw new InvalidOperationException("User not found in context.");
}