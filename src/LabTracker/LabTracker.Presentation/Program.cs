using Hellang.Middleware.ProblemDetails;
using LabTracker.Application;
using LabTracker.Application.Contracts;
using LabTracker.Application.Contracts.Labs;
using LabTracker.Application.Courses;
using LabTracker.Application.Labs;
using LabTracker.Application.Notifications;
using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Identity;
using LabTracker.Infrastructure.Persistence;
using LabTracker.Infrastructure.Persistence.Repositories;
using LabTracker.Infrastructure.Services;
using LabTracker.Presentation;
using LabTracker.Presentation.Dtos;
using LabTracker.Presentation.Endpoints;
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
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILabRepository, LabRepository>();

// Add our services.
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILabService, LabService>();


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

api.MapAuthEndpoints();

api.MapUserEndpoints();

api.MapCourseEndpoints();

app.MapNotificationEndpoints();

app.MapLabEndpoints();

app.Run();