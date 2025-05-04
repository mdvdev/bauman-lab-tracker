using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LabTracker.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    // Additional configuration to IdentityUser.
    public void Configure(EntityTypeBuilder<User> builder)
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