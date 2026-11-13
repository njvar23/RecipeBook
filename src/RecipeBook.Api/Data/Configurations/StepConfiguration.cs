using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

/// <summary>1:N — Recipe → Step. Steps live and die with their recipe.</summary>
public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> b)
    {
        b.HasKey(s => s.Id);
        b.Property(s => s.Instruction).HasMaxLength(500).IsRequired();

        b.HasOne(s => s.Recipe)
         .WithMany(r => r.Steps)
         .HasForeignKey(s => s.RecipeId)
         .OnDelete(DeleteBehavior.Cascade); // deleting a recipe deletes its steps

        // One recipe cannot have two "step 2"s
        b.HasIndex(s => new { s.RecipeId, s.Order }).IsUnique();
    }
}
