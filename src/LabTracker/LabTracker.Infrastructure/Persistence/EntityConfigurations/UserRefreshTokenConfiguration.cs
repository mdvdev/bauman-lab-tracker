using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.ToTable("user_refresh_tokens");
        
        builder.HasKey(t => t.Id);
        
        builder.HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<UserRefreshToken>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(t => t.RefreshToken)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(t => t.ExpiresAt)
            .IsRequired();
    }
}