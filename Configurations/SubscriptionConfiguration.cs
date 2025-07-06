using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => s.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(s => s.StartDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.NextPaymentDate)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(20); // e.g., "active", "cancelled", "paused", "failed"

            builder.Property(s => s.PaymentGatewaySubscriptionId)
                .IsRequired()
                .HasMaxLength(255); // Store the ID from Stripe/PayPal/etc.

            builder.HasIndex(s => s.PaymentGatewaySubscriptionId)
                .IsUnique(); // Ensure unique IDs from payment gateway

            // Relationships
            builder.HasOne(s => s.Buyer)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.BuyerId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a user if they have active subscriptions

            builder.HasOne(s => s.Product)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a product if it has active subscriptions
        }

    }
}
