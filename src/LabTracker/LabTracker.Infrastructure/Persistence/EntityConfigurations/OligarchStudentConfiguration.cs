using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class OligarchStudentConfiguration : IEntityTypeConfiguration<OligarchStudentEntity>
{
    public void Configure(EntityTypeBuilder<OligarchStudentEntity> builder)
    {
        builder.ToTable("OligarchStudents");

        builder.HasKey(s => new { s.UserId, s.CourseId });

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.CourseId)
            .IsRequired();

        builder.Property(s => s.GrantedAt)
            .IsRequired();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}