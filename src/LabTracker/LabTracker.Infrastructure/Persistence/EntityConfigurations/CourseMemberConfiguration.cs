using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class CourseMemberConfiguration : IEntityTypeConfiguration<CourseMemberEntity>
{
    public void Configure(EntityTypeBuilder<CourseMemberEntity> builder)
    {
        builder.ToTable("CourseMembers");

        builder.HasKey(cm => new { cm.CourseId, cm.MemberId });

        builder.HasOne(cm => cm.Course)
            .WithMany()
            .HasForeignKey(cm => cm.CourseId);

        builder.HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.MemberId);

        builder.Property(cm => cm.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(cm => cm.Score);
    }
}