﻿namespace ITI_Raqmiya_MVC.Models
{
    public class Product
    {
        public int Id { get; set; } // Primary Key
        public int CreatorId { get; set; } // Foreign Key to User
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty; // e.g., "USD", "EUR"
        public string ProductType { get; set; } = string.Empty; // e.g., "digital_download", "subscription", "course"
        public string? CoverImageUrl { get; set; } // Nullable
        public string? PreviewVideoUrl { get; set; } // Nullable
        public DateTime? PublishedAt { get; set; } // Nullable, if not published yet
        public string Status { get; set; } = "draft"; // e.g., "draft", "published", "archived", "unlisted"
        public bool IsPublic { get; set; }
        public string Permalink { get; set; } = string.Empty;

        // Navigation properties
        public User Creator { get; set; } = null!; // Required navigation property
        public ICollection<AddedFile> Files { get; set; } = new List<AddedFile>();
        public ICollection<Order> Orders { get; set; } = new List<Order>(); // Products can be in many orders
        public ICollection<Variant> Variants { get; set; } = new List<Variant>();
        public ICollection<OfferCode> OfferCodes { get; set; } = new List<OfferCode>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();


    }
}
