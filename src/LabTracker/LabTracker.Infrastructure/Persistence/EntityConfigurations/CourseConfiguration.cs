using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class CourseConfiguration : IEntityTypeConfiguration<CourseEntity>
{
    public void Configure(EntityTypeBuilder<CourseEntity> builder)
    {
        builder.ToTable("Courses");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.QueueMode)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.PhotoUri)
            .HasMaxLength(500);
    }
}