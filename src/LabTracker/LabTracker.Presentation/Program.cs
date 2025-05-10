using System.Text.Json.Serialization;
using Hellang.Middleware.ProblemDetails;
using LabTracker.Application.Auth;
using LabTracker.Application.Contracts;
using LabTracker.Application.Courses;
using LabTracker.Application.Users;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Identity;
using LabTracker.Infrastructure.Persistence;
using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Infrastructure.Persistence.Repositories;
using LabTracker.Infrastructure.Services;
using LabTracker.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) =>
        builder.Environment.IsDevelopment();

    options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<NotSupportedException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<InvalidOperationException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);
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
    options.AddPolicy("TeacherOrAdmin", policy =>
        policy.RequireRole(nameof(Role.Teacher), nameof(Role.Administrator)));
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<UserEntity>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add repositories.
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseMemberRepository, CourseMemberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileValidator, ImageFileValidator>();
builder.Services.AddScoped<ImageFileValidator>();
builder.Services.AddScoped<IFileValidatorFactory, FileValidatorFactory>();
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

app.MapControllers();

app.Run();