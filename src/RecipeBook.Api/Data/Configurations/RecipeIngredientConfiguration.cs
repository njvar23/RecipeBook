using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

/// <summary>
/// N:M with extra columns — Recipe ↔ Ingredient through RecipeIngredient.
/// Because the link has its own data (Quantity, Unit) it is an explicit entity
/// with a composite primary key, not EF's automatic join table.
/// </summary>
public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> b)
    {
        b.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        b.Property(ri => ri.Quantity).HasPrecision(10, 2);
        b.Property(ri => ri.Unit).HasConversion<string>().HasMaxLength(10).IsRequired();

        b.HasOne(ri => ri.Recipe)
         .WithMany(r => r.RecipeIngredients)
         .HasForeignKey(ri => ri.RecipeId)
         .OnDelete(DeleteBehavior.Cascade);  // links disappear with the recipe

        b.HasOne(ri => ri.Ingredient)
         .WithMany(i => i.RecipeIngredients)
         .HasForeignKey(ri => ri.IngredientId)
         .OnDelete(DeleteBehavior.Restrict); // an ingredient used in recipes cannot be deleted
    }
}
