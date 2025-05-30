using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<SubmissionEntity>
{
    public void Configure(EntityTypeBuilder<SubmissionEntity> builder)
    {
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SubmissionStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Submissions_Students_StudentId");

        builder.HasOne(s => s.Lab)
            .WithMany()
            .HasForeignKey(s => s.LabId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Submissions_Labs_LabId");

        builder.HasOne(s => s.Slot)
            .WithMany()
            .HasForeignKey(s => s.SlotId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Submissions_Slots_SlotId");

        builder.HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Submissions_Courses_CourseId");

        builder.HasIndex(s => s.StudentId);
        builder.HasIndex(s => s.LabId);
        builder.HasIndex(s => s.SlotId);
        builder.HasIndex(s => s.CourseId);
    }
}