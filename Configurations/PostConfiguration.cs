using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)"); // For potentially very long content

            builder.Property(p => p.PublishedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.IsPublic)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Creator)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Cascade); // If creator is deleted, their posts are deleted

            builder.HasOne(p => p.Product)
                .WithMany() // A post isn't inherently 'part' of a product's collection, but references one
                .HasForeignKey(p => p.ProductId)
                .IsRequired(false) // ProductId is nullable
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of product if it has related posts
        }
    }
}
