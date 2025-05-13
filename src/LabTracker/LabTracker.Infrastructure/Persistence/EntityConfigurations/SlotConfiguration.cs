using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class SlotConfiguration : IEntityTypeConfiguration<SlotEntity>
{
    public void Configure(EntityTypeBuilder<SlotEntity> builder)
    {
        builder.ToTable("Slots");

        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Course)
            .WithMany()
            .HasForeignKey(s => s.CourseId);

        builder.HasOne(s => s.Teacher)
            .WithMany()
            .HasForeignKey(s => s.TeacherId);
        
        builder.Property(s => s.StartTime)
            .IsRequired();
        
        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.MaxStudents)
            .IsRequired();
    }
}