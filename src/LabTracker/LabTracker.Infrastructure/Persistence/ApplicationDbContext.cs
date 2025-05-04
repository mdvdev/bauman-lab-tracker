using LabTracker.Domain.Entities;
using LabTracker.Domain.ValueObjects;
using LabTracker.Infrastructure.Persistence.Converters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseMember> CourseMembers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<Name>()
            .HaveConversion<NameConverter>()
            .HaveMaxLength(100);
    }
}