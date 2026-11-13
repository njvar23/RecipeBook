using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(u => u.Id);

        b.Property(u => u.Email).HasMaxLength(256).IsRequired();
        b.HasIndex(u => u.Email).IsUnique();

        b.Property(u => u.PasswordHash).HasMaxLength(100).IsRequired();
        b.Property(u => u.DisplayName).HasMaxLength(50).IsRequired();

        // Store the enum as readable text ("User" / "Admin") instead of 0 / 1
        b.Property(u => u.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
    }
}
