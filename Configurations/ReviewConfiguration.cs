using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(r => r.Rating)
                .IsRequired()
                .HasColumnType("int"); // Ensure it's an integer
                                       // You might add a check constraint in raw SQL for 1-5 rating if desired,
                                       // but EF Core won't generate it directly.

            builder.Property(r => r.Comment)
                .HasMaxLength(2000); // Max length for review comments

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.IsApproved)
                .IsRequired()
                .HasDefaultValue(false); // Default to not approved, requiring moderation

            // Relationships
            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // If a product is deleted, its reviews are also deleted

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete a user if they have left reviews (or you could cascade, depending on business logic)
                                                    // If you set to Cascade, then reviews by a deleted user are removed.
                                                    // Restrict means you must delete the reviews first or change their UserId to null/anonymous.
        }
    }
}


