using LabTracker.Application.Courses;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;

namespace LabTracker.Presentation.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var courses = endpoints
            .MapGroup("/courses")
            .WithTags("Courses");

        courses.MapGet("/", async (
            HttpContext context, ICourseService courseService) =>
        {
            if (context.Items[ContextKeys.CurrentUser] is not User user)
                return Results.NotFound();

            var courseMembers = await courseService.GetMemberCoursesAsync(user.Id);
            return Results.Ok(courseMembers
                .Select(cm => new { cm.Course, cm.AssignedAt })
                .ToList());
        });


        // TODO: Implement this method.
        courses.MapPost("/", async (
                HttpContext context, ICourseService courseService) =>
            {
            })
            .RequireAuthorization(nameof(Role.Teacher), nameof(Role.Administrator));

        return endpoints;
    }
}