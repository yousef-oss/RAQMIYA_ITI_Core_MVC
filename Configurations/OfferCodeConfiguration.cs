using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class OfferCodeConfiguration : IEntityTypeConfiguration<OfferCode>
    {
        public void Configure(EntityTypeBuilder<OfferCode> builder)
        {
            builder.HasKey(oc => oc.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(oc => oc.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(oc => oc.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(oc => oc.Code)
                .IsUnique(); // Ensure offer codes are unique

            builder.Property(oc => oc.DiscountType)
                .IsRequired()
                .HasMaxLength(20); // e.g., "percentage", "fixed_amount"

            builder.Property(oc => oc.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18, 2)"); // For percentage (0-100) or fixed amount

            builder.Property(oc => oc.TimesUsed)
                .IsRequired()
                .HasDefaultValue(0); // Start at 0

            builder.Property(oc => oc.StartsAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(oc => oc.Product)
                .WithMany(p => p.OfferCodes)
                .HasForeignKey(oc => oc.ProductId)
                .IsRequired(false) // ProductId can be null for store-wide codes
                .OnDelete(DeleteBehavior.Cascade); // If the product is deleted, its specific offer codes are deleted
        }
    }
}


