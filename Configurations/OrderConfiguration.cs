using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(o => o.PricePaid)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(o => o.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(o => o.TransactionId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.OrderedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(o => o.GuestEmail)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(o => o.Buyer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.BuyerId)
                .IsRequired(false) // BuyerId can be null for guest checkouts
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user if they have orders

            builder.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a product if it has associated orders

            // One-to-one relationship with License
            builder.HasOne(o => o.License)
                .WithOne(l => l.Order)
                .HasForeignKey<License>(l => l.OrderId) // License table has the FK to Order
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If an order is deleted, its license is deleted
        }
    }
}
