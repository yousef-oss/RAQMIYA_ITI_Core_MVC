using ITI_Raqmiya_MVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace ITI_Raqmiya_MVC.Data
{
    public class RaqmiyaContext : DbContext
    {
        public RaqmiyaContext(DbContextOptions<RaqmiyaContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<AddedFile> Files { get; set; } = null!;
        public DbSet<License> Licenses { get; set; } = null!;
        public DbSet<Variant> Variants { get; set; } = null!;
        public DbSet<OfferCode> OfferCodes { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = null!;


        // NEW DbSets for Tags, Wishlist, and Views
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<CategoryTag> CategoryTags { get; set; } = null!;
        public DbSet<ProductTag> ProductTags { get; set; } = null!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = null!;
        public DbSet<ProductView> ProductViews { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the current assembly.
            // This assumes your configuration classes (e.g., UserConfiguration)
            // are in the same assembly as your DbContext or referenced correctly.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // You can also apply specific configurations manually if preferred:
            // modelBuilder.ApplyConfiguration(new UserConfiguration());
            // modelBuilder.ApplyConfiguration(new ProductConfiguration());
            // ... and so on for other complex configurations

        }
    }
}
