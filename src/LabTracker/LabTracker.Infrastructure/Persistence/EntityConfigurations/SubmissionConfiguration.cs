using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");
        
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Score)
            .HasDefaultValue(null);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Lab)
            .WithMany()
            .HasForeignKey(s => s.LabId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Slot)
            .WithMany()
            .HasForeignKey(s => s.SlotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.StudentId);
        builder.HasIndex(s => s.LabId);
        builder.HasIndex(s => s.SlotId);
        builder.HasIndex(s => s.CourseId);
    }
}