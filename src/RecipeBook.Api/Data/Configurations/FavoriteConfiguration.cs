using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

/// <summary>N:M with a date — User ↔ Recipe through Favorite.</summary>
public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> b)
    {
        // Composite key = the same recipe cannot be favorited twice by the same user
        b.HasKey(f => new { f.UserId, f.RecipeId });

        b.HasOne(f => f.User)
         .WithMany(u => u.Favorites)
         .HasForeignKey(f => f.UserId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(f => f.Recipe)
         .WithMany(r => r.Favorites)
         .HasForeignKey(f => f.RecipeId)
         .OnDelete(DeleteBehavior.Cascade); // deleting a recipe removes it from everyone's favorites
    }
}
