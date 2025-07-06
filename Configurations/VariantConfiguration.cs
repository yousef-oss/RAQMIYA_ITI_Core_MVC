using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class VariantConfiguration : IEntityTypeConfiguration<Variant>
    {
        public void Configure(EntityTypeBuilder<Variant> builder)
        {
            builder.HasKey(v => v.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.PriceAdjustment)
                .IsRequired()
                .HasColumnType("decimal(18, 2)"); // Consistent decimal precision

            builder.Property(v => v.Description)
                .HasMaxLength(500); // Optional, so nullable in DB

            // Relationships
            builder.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If a product is deleted, its variants should also be deleted
        }
    }
}


