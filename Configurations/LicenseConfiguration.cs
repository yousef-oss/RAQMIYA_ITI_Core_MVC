using Microsoft.EntityFrameworkCore;
using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class LicenseConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.HasKey(l => l.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(l => l.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(l => l.LicenseKey)
                .HasMaxLength(100);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(l => l.AccessGrantedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationships (Note: One-to-one is often configured from the dependent side, which is License)
            builder.HasOne(l => l.Order)
                .WithOne(o => o.License)
                .HasForeignKey<License>(l => l.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If the order is deleted, the license is also deleted

            builder.HasOne(l => l.Product)
                .WithMany() // Licenses don't "own" products, but refer to them
                .HasForeignKey(l => l.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a product if it has active licenses

            builder.HasOne(l => l.Buyer)
                .WithMany() // Licenses don't "own" users, but refer to them
                .HasForeignKey(l => l.BuyerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // If a buyer is deleted, their licenses are deleted
        }
    }
}

