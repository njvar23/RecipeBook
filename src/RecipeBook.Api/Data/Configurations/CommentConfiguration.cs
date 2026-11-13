using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Text).HasMaxLength(500).IsRequired();

        // 1:N Recipe → Comment. Deleting a recipe deletes its comments.
        b.HasOne(c => c.Recipe)
         .WithMany(r => r.Comments)
         .HasForeignKey(c => c.RecipeId)
         .OnDelete(DeleteBehavior.Cascade);

        // 1:N User → Comment. Restrict so there is only one cascade path into Comments.
        b.HasOne(c => c.User)
         .WithMany(u => u.Comments)
         .HasForeignKey(c => c.UserId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
