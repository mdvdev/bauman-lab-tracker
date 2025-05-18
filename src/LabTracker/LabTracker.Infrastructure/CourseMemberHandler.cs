using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Domain;
using LabTracker.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LabTracker.Infrastructure;

public class CourseMemberHandler : AuthorizationHandler<CourseMemberRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICourseMemberService _courseMemberService;
    private readonly ICurrentUserService _currentUserService;

    public CourseMemberHandler(
        IHttpContextAccessor httpContextAccessor,
        ICourseMemberService courseMemberService,
        ICurrentUserService currentUserService)
    {
        _httpContextAccessor = httpContextAccessor;
        _courseMemberService = courseMemberService;
        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CourseMemberRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var courseIdValue = routeData?.Values["courseId"]?.ToString();

        if (!Guid.TryParse(courseIdValue, out var courseId))
            return;

        var key = new CourseMemberKey(courseId, _currentUserService.User.Id);
        if (await _courseMemberService.IsCourseMemberAsync(key))
            context.Succeed(requirement);
    }
}