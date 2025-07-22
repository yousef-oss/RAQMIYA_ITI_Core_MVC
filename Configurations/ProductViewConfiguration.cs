using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class ProductViewConfiguration : IEntityTypeConfiguration<ProductView>
    {
        public void Configure(EntityTypeBuilder<ProductView> builder)
        {
            builder.HasKey(pv => pv.Id);
            builder.Property(pv => pv.Id).ValueGeneratedOnAdd();

            builder.Property(pv => pv.ViewedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pv => pv.IpAddress)
                .HasMaxLength(45); // Sufficient for IPv6

            builder.HasOne(pv => pv.Product)
                .WithMany(p => p.ProductViews)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If product deleted, remove its view logs

            builder.HasOne(pv => pv.User)
                .WithMany(u => u.ProductViews)
                .HasForeignKey(pv => pv.UserId)
                .IsRequired(false) // UserId is nullable for guest views
                .OnDelete(DeleteBehavior.SetNull); // If user deleted, their views remain but become anonymous
        }
    }
}
