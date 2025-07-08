using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class CategoryTagConfiguration : IEntityTypeConfiguration<CategoryTag>
    {
        public void Configure(EntityTypeBuilder<CategoryTag> builder)
        {
            builder.HasKey(ct => new { ct.CategoryId, ct.TagId }); // Composite PK

            builder.HasOne(ct => ct.Category)
                .WithMany(c => c.CategoryTags)
                .HasForeignKey(ct => ct.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // If category deleted, remove its tag associations

            builder.HasOne(ct => ct.Tag)
                .WithMany(t => t.CategoryTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade); // If tag deleted, remove its category associations
        }
    }
}
