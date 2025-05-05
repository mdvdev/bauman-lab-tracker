using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations
{
    public class LabConfiguration : IEntityTypeConfiguration<Lab>
    {
        public void Configure(EntityTypeBuilder<Lab> builder)
        {
            builder.ToTable("Labs");
            
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(l => l.Deadline)
                .IsRequired();

            builder.Property(l => l.Score)
                .IsRequired();

            builder.Property(l => l.ScoreAfterDeadline)
                .IsRequired();

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}