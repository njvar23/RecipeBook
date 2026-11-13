using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.Name).HasMaxLength(50).IsRequired();
        b.HasIndex(i => i.Name).IsUnique();

        b.HasData(LookupSeedData.Ingredients);
    }
}
