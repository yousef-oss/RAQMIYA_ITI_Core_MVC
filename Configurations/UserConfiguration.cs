using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITI_Raqmiya_MVC.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(u => u.Id);

            // Configure Id as identity (auto-increment)
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            // Properties
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255); // Max length for email

            builder.HasIndex(u => u.Email) // Ensure email is unique
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255); // Adjust length based on hashing algorithm

            //Salt configuration
            builder.Property(u => u.Salt)
                .IsRequired()
                .HasMaxLength(128); // A common length for salts, adjust based on your hashing needs (e.g., 64, 128, 256)

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(u => u.Username) // Ensure username is unique
                .IsUnique();

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            builder.Property(u => u.IsCreator)
                .IsRequired();

            builder.Property(u => u.ProfileDescription)
                .HasMaxLength(1000); // Max length for description

            builder.Property(u => u.ProfileImageUrl)
                .HasMaxLength(500); // Max length for URL

            builder.Property(u => u.StripeConnectAccountId)
                .HasMaxLength(100);

            // Relationships
            builder.HasMany(u => u.Products)
                .WithOne(p => p.Creator)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a user if they have products

            builder.HasMany(u => u.Orders)
                .WithOne(o => o.Buyer)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user if they have orders

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete reviews if user is deleted

            builder.HasMany(u => u.Subscriptions)
                .WithOne(s => s.Buyer)
                .HasForeignKey(s => s.BuyerId)
                .OnDelete(DeleteBehavior.Cascade); // Delete subscriptions if user is deleted

            builder.HasMany(u => u.Posts)
                .WithOne(p => p.Creator)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Cascade); // Delete posts if creator is deleted
        }
    }
}

