using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ITI_Raqmiya_MVC.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(p => p.ProductType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Permalink)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(p => p.Permalink)
                .IsUnique();

            // Relationships
            builder.HasOne(p => p.Creator)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Defined in UserConfiguration too, ensure consistency


            builder.HasMany(p => p.Files)
                .WithOne(f => f.Product)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete files if product is deleted

            builder.HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete variants if product is deleted

            builder.HasMany(p => p.OfferCodes)
                .WithOne(oc => oc.Product)
                .HasForeignKey(oc => oc.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete offer codes if specific product is deleted

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete reviews if product is deleted

            builder.HasMany(p => p.Subscriptions)
                .WithOne(s => s.Product)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete product if active subscriptions exist

            builder.HasMany(p => p.Orders)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete product if it has orders

            // NEW: Many-to-many relationship with Category via ProductCategory join entity
            builder.HasMany(p => p.ProductCategories) // A Product has many ProductCategory entries
                .WithOne(pc => pc.Product) // Each ProductCategory entry is for one Product
                .HasForeignKey(pc => pc.ProductId) // The foreign key in ProductCategory pointing to Product is ProductId
                .OnDelete(DeleteBehavior.Cascade); // If a product is deleted, its entries in the join table are deleted
        }
    }
}

 
