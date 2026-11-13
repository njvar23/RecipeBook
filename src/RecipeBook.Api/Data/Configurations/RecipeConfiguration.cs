using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> b)
    {
        b.HasKey(r => r.Id);

        b.Property(r => r.Title).HasMaxLength(150).IsRequired();
        b.Property(r => r.Description).HasMaxLength(2000);
        b.Property(r => r.Difficulty).HasConversion<string>().HasMaxLength(10).IsRequired();

        // 1:N Category → Recipe. Restrict: a category that still has recipes cannot be deleted.
        b.HasOne(r => r.Category)
         .WithMany(c => c.Recipes)
         .HasForeignKey(r => r.CategoryId)
         .OnDelete(DeleteBehavior.Restrict);

        // 1:N User → Recipe. Restrict: we never silently delete a user's recipes.
        // (Also avoids "multiple cascade paths" on SQL Server: User → Recipe → Comment and User → Comment.)
        b.HasOne(r => r.Author)
         .WithMany(u => u.Recipes)
         .HasForeignKey(r => r.AuthorId)
         .OnDelete(DeleteBehavior.Restrict);

        // Indexes for the most common list filters
        b.HasIndex(r => r.CategoryId);
        b.HasIndex(r => r.AuthorId);
        b.HasIndex(r => r.CreatedAt);
    }
}
