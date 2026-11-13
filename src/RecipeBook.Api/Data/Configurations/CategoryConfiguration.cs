using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Name).HasMaxLength(50).IsRequired();
        b.HasIndex(c => c.Name).IsUnique();

        // Lookup seed data goes into the migration itself
        b.HasData(LookupSeedData.Categories);
    }
}
