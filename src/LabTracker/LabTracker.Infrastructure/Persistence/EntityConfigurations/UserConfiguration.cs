using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    // Additional configuration to IdentityUser.
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Patronymic)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.TelegramUsername)
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.PhotoUri);
    }
}