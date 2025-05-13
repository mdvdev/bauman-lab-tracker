using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<CourseMemberEntity> CourseMembers { get; set; }
    public DbSet<LabEntity> Labs { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<SubmissionEntity> Submissions { get; set; }
    public DbSet<SubmissionEntity> Slots { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}