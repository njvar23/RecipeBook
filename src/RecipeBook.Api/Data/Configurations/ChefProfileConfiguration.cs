using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

/// <summary>1:1 — User ↔ ChefProfile (the profile is optional).</summary>
public class ChefProfileConfiguration : IEntityTypeConfiguration<ChefProfile>
{
    public void Configure(EntityTypeBuilder<ChefProfile> b)
    {
        b.HasKey(p => p.Id);

        b.Property(p => p.Bio).HasMaxLength(1000);
        b.Property(p => p.Speciality).HasMaxLength(80).IsRequired();

        b.HasOne(p => p.User)                       // a profile belongs to one user
         .WithOne(u => u.ChefProfile)               // a user has at most one profile
         .HasForeignKey<ChefProfile>(p => p.UserId) // FK lives on the dependent side (ChefProfile)
         .OnDelete(DeleteBehavior.Cascade);         // profile disappears with its user

        // This unique index is what makes the relationship 1:1 at database level
        b.HasIndex(p => p.UserId).IsUnique();
    }
}
