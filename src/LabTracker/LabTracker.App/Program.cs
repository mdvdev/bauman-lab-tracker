using System.Text.Json.Serialization;
using Auth.Services;
using Auth.Web;
using Courses.Services;
using Courses.Web;
using Hellang.Middleware.ProblemDetails;
using Labs.Services;
using Labs.Web;
using LabTracker.Auth.Abstractions.Services;
using LabTracker.CourseMembers.Abstractions.Repositories;
using LabTracker.CourseMembers.Abstractions.Services;
using LabTracker.CourseMembers.Services;
using LabTracker.CourseMembers.Web;
using LabTracker.Courses.Abstractions.Repositories;
using LabTracker.Courses.Abstractions.Services;
using LabTracker.Infrastructure;
using LabTracker.Infrastructure.Identity;
using LabTracker.Infrastructure.Persistence;
using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Infrastructure.Persistence.Repositories;
using LabTracker.Infrastructure.Services;
using LabTracker.Labs.Abstractions.Repositories;
using LabTracker.Labs.Abstractions.Services;
using LabTracker.Notifications.Abstractions.Repositories;
using LabTracker.Notifications.Abstractions.Services;
using LabTracker.Shared.Contracts;
using LabTracker.Slots.Abstractions.Repositories;
using LabTracker.Slots.Abstractions.Services;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Abstractions.Services;
using LabTracker.Submissions.Services;
using LabTracker.Submissions.Web;
using LabTracker.User.Abstractions.Repositories;
using LabTracker.User.Abstractions.Services;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Notifications.Services;
using Notifications.Web;
using Shared;
using Slots.Services;
using Slots.Web;
using Users.Services;
using Users.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) =>
        builder.Environment.IsDevelopment();

    options.MapToStatusCode<ArgumentException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<ArgumentOutOfRangeException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<NotSupportedException>(StatusCodes.Status400BadRequest);
    options.MapToStatusCode<InvalidOperationException>(StatusCodes.Status400BadRequest);

    options.MapToStatusCode<UnauthorizedAccessException>(StatusCodes.Status401Unauthorized);

    options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(nameof(Role.Administrator), policy => { policy.RequireRole(nameof(Role.Administrator)); });
    
    options.AddPolicy(nameof(Role.Teacher), policy => { policy.RequireRole(nameof(Role.Teacher)); });

    options.AddPolicy("TeacherOrAdmin", policy =>
        policy.RequireRole(nameof(Role.Teacher), nameof(Role.Administrator)));

    options.AddPolicy("CourseMemberOnly", policy =>
        policy.Requirements.Add(new CourseMemberRequirement()));

    options.AddPolicy("TeacherAndCourseMember", policy =>
    {
        policy.RequireRole(nameof(Role.Teacher));
        policy.Requirements.Add(new CourseMemberRequirement());
    });
    
    options.AddPolicy("StudentAndCourseMember", policy =>
    {
        policy.RequireRole(nameof(Role.Student));
        policy.Requirements.Add(new CourseMemberRequirement());
    });

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

builder.Services
    .AddIdentity<UserEntity, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthController).Assembly)
    .AddApplicationPart(typeof(CourseStudentController).Assembly)
    .AddApplicationPart(typeof(CourseTeacherController).Assembly)
    .AddApplicationPart(typeof(CourseController).Assembly)
    .AddApplicationPart(typeof(LabController).Assembly)
    .AddApplicationPart(typeof(NotificationController).Assembly)
    .AddApplicationPart(typeof(SlotController).Assembly)
    .AddApplicationPart(typeof(SubmissionController).Assembly)
    .AddApplicationPart(typeof(UserController).Assembly)
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// Add repositories.
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseMemberRepository, CourseMemberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILabRepository, LabRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();

// Add services.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseMemberService, CourseMemberService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILabService, LabService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<ISlotService, SlotService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IFileValidator, ImageFileValidator>();
builder.Services.AddScoped<ImageFileValidator>();
builder.Services.AddScoped<IFileValidatorFactory, FileValidatorFactory>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IAuthorizationHandler, CourseMemberHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(services);
    //await IdentitySeeder.SeedAdminAsync(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseStaticFiles();

app.UseProblemDetails();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CurrentUserMiddleware>();

var staticFilesPath = Path.Combine(builder.Environment.ContentRootPath, "StaticFiles");

if (!Directory.Exists(staticFilesPath))
{
    Directory.CreateDirectory(staticFilesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesPath),
    RequestPath = "/StaticFiles"
});

app.MapControllers();

app.Run();