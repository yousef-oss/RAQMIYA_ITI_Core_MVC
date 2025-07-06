using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<AddedFile>
    {
        public void Configure(EntityTypeBuilder<AddedFile> builder)
        {
            builder.HasKey(f => f.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.FileUrl)
                .IsRequired()
                .HasMaxLength(500); // Adjust based on max URL length for your storage

            builder.Property(f => f.Size)
                .IsRequired();

            builder.Property(f => f.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            // Relationship
            builder.HasOne(f => f.Product)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete files if product is deleted
        }
    }
}


