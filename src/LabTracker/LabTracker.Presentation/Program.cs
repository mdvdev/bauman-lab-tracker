using Hellang.Middleware.ProblemDetails;
using LabTracker.Application.Contracts;
using LabTracker.Application.Courses;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Identity;
using LabTracker.Infrastructure.Persistence;
using LabTracker.Infrastructure.Persistence.Repositories;
using LabTracker.Presentation;
using LabTracker.Presentation.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) =>
        builder.Environment.IsDevelopment();

    options.MapToStatusCode<InvalidOperationException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
    options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Role.Administrator), policy => { policy.RequireRole(nameof(Role.Administrator)); });

    options.AddPolicy(nameof(Role.Teacher), policy => { policy.RequireRole(nameof(Role.Teacher)); });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add repositories.
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseMemberRepository, CourseMemberRepository>();

// Add our services.
builder.Services.AddScoped<ICourseService, CourseService>();


var app = builder.Build();


app.UseProblemDetails();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(services);
    //await IdentitySeeder.SeedAdminAsync(services);
}

// TODO: Add email service.

var api = app
    .MapGroup("/api")
    .WithTags("Api");


var auth = api
    .MapGroup("/auth")
    .WithTags("Auth");


auth.MapCustomizedIdentityApi<User>();


auth.MapPost("/logout", async (SignInManager<User> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    })
    .RequireAuthorization();


auth.MapPatch("/update-password", async (
        HttpContext context, UserManager<User> userManager, UpdateUserPasswordDto dto) =>
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null) return Results.Unauthorized();

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        return !result.Succeeded ? Results.BadRequest() : Results.Ok();
    })
    .RequireAuthorization();


var users = api
    .MapGroup("/users")
    .WithTags("Users");


users.MapGet("/", async (UserManager<User> userManager) =>
    {
        var userList = await userManager.Users.ToListAsync();

        var result = new List<UserDto>();
        foreach (var user in userList)
        {
            var rolesString = await userManager.GetRolesAsync(user);
            var roles = rolesString.Select(Enum.Parse<Role>).ToList();
            result.Add(UserDto.Create(user, roles));
        }

        return Results.Ok(result);
    })
    .RequireAuthorization(nameof(Role.Administrator));


users.MapGet("/me", async (
        HttpContext context, UserManager<User> userManager) =>
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null) return Results.NotFound();

        var rolesString = await userManager.GetRolesAsync(user);
        var roles = rolesString.Select(Enum.Parse<Role>).ToList();

        return Results.Ok(UserDto.Create(user, roles));
    })
    .RequireAuthorization();


users.MapPatch("/me", async (
        HttpContext context, UserManager<User> userManager, UpdateUserDto dto) =>
    {
        if (dto.FirstName is null &&
            dto.LastName is null &&
            dto.Patronymic is null &&
            dto.TelegramUsername is null &&
            dto.PhotoUri is null)
        {
            return Results.BadRequest("At least one field must be provided.");
        }

        var user = await userManager.GetUserAsync(context.User);

        if (user is null) return Results.NotFound();

        user.FirstName = dto.FirstName != null ? new Name(dto.FirstName) : user.FirstName;
        user.LastName = dto.LastName != null ? new Name(dto.LastName) : user.LastName;
        user.Patronymic = dto.Patronymic != null ? new Name(dto.Patronymic) : user.Patronymic;
        user.TelegramUsername = dto.TelegramUsername ?? user.TelegramUsername;
        user.PhotoUri = dto.PhotoUri ?? user.PhotoUri;

        await userManager.UpdateAsync(user);

        return Results.Ok();
    })
    .RequireAuthorization();


var courses = api
    .MapGroup("/courses")
    .WithTags("Courses");


courses.MapGet("/", async (
        HttpContext context, UserManager<User> userManager, ICourseService courseService) =>
    {
        var user = await userManager.GetUserAsync(context.User);
        if (user is null) return Results.NotFound();

        var courseMembers = await courseService.GetMemberCoursesAsync(user.Id);
        return Results.Ok(courseMembers
            .Select(cm => new { cm.Course, cm.AssignedAt })
            .ToList());
    })
    .RequireAuthorization();


courses.MapPost("/", async (
        HttpContext context, UserManager<User> userManager, ICourseService courseService) =>
    {
    })
    .RequireAuthorization(nameof(Role.Teacher), nameof(Role.Administrator));


app.Run();