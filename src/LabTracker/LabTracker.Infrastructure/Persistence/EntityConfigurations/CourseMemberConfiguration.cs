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

        builder.HasOne(ct => ct.Course)
            .WithMany()
            .HasForeignKey(ct => ct.CourseId);

        builder.HasOne(ct => ct.User)
            .WithMany()
            .HasForeignKey(ct => ct.MemberId);

        builder.Property(ct => ct.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}