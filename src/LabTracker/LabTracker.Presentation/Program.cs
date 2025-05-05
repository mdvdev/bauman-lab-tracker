using Hellang.Middleware.ProblemDetails;
using LabTracker.Application.Contracts;
using LabTracker.Application.Courses;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Identity;
using LabTracker.Infrastructure.Persistence;
using LabTracker.Infrastructure.Persistence.Repositories;
using LabTracker.Infrastructure.Services;
using LabTracker.Presentation;
using LabTracker.Presentation.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
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
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(services);
    //await IdentitySeeder.SeedAdminAsync(services);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseProblemDetails();
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    RequestPath = "/StaticFiles"
});

app.UseMiddleware<CurrentUserMiddleware>();

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
});


auth.MapPatch("/password", async (
    HttpContext context, UserManager<User> userManager, UpdateUserPasswordDto dto) =>
{
    if (context.Items[ContextKeys.CurrentUser] is not User user)
        return Results.NotFound();

    var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

    return !result.Succeeded ? Results.BadRequest() : Results.Ok();
});


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
}).RequireAuthorization(nameof(Role.Administrator));


users.MapGet("/me", async (
    HttpContext context, UserManager<User> userManager) =>
{
    if (context.Items[ContextKeys.CurrentUser] is not User user)
        return Results.NotFound();

    var rolesString = await userManager.GetRolesAsync(user);
    var roles = rolesString.Select(Enum.Parse<Role>).ToList();

    return Results.Ok(UserDto.Create(user, roles));
});


users.MapPatch("/me", async (
    HttpContext context, UserManager<User> userManager, UpdateUserProfileDto dto) =>
{
    if (dto.FirstName is null &&
        dto.LastName is null &&
        dto.Patronymic is null &&
        dto.TelegramUsername is null)
    {
        return Results.BadRequest("At least one field must be provided.");
    }

    if (context.Items[ContextKeys.CurrentUser] is not User user)
        return Results.NotFound();

    user.FirstName = dto.FirstName != null ? new Name(dto.FirstName) : user.FirstName;
    user.LastName = dto.LastName != null ? new Name(dto.LastName) : user.LastName;
    user.Patronymic = dto.Patronymic != null ? new Name(dto.Patronymic) : user.Patronymic;
    user.TelegramUsername = dto.TelegramUsername ?? user.TelegramUsername;

    await userManager.UpdateAsync(user);

    return Results.Ok();
});


users.MapPatch("/me/photo", async (
    HttpContext context, UserManager<User> userManager, IFileService fileService, IFormFile file) =>
{
    try
    {
        var filePath = await fileService.SaveImageAsync(file,
            "StaticFiles/Images/ProfilePhotos/",
            Path.GetFileNameWithoutExtension(file.FileName));

        if (context.Items[ContextKeys.CurrentUser] is not User user)
            return Results.NotFound();

        user.PhotoUri = filePath;
        await userManager.UpdateAsync(user);

        return Results.Ok();
    }
    catch (InvalidOperationException e)
    {
        return Results.BadRequest(e.Message);
    }
}).DisableAntiforgery();


var courses = api
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


app.Run();