using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.Name)
                .IsUnique(); // Ensure tag names are unique

            // Relationships (configured in join tables)
            builder.HasMany(t => t.CategoryTags)
                .WithOne(ct => ct.Tag)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade); // If a tag is deleted, remove its associations with categories

            builder.HasMany(t => t.ProductTags)
                .WithOne(pt => pt.Tag)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade); // If a tag is deleted, remove its associations with products
        }
    }
}
