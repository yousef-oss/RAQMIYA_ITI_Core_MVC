using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasKey(wi => wi.Id);
            builder.Property(wi => wi.Id).ValueGeneratedOnAdd();

            // Ensure a user can only wish for a product once (unique constraint)
            builder.HasIndex(wi => new { wi.UserId, wi.ProductId }).IsUnique();

            builder.Property(wi => wi.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(wi => wi.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(wi => wi.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user deleted, remove their wishlist items

            builder.HasOne(wi => wi.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If product deleted, remove it from all wishlists
        }
    }
}
