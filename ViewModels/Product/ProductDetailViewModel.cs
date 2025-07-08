namespace ITI_Raqmiya_MVC.ViewModels.Product
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string CreatorUsername { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string? PreviewVideoUrl { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string Permalink { get; set; } = string.Empty;

        public List<FileViewModel> Files { get; set; } = new List<FileViewModel>();
        public List<VariantViewModel> Variants { get; set; } = new List<VariantViewModel>();
        public List<OfferCodeViewModel> OfferCodes { get; set; } = new List<OfferCodeViewModel>();
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<TagViewModel> Tags { get; set; } = new List<TagViewModel>();

        public int WishlistCount { get; set; }
        public double AverageRating { get; set; }
        public int SalesCount { get; set; }
        public int ViewsCount { get; set; }

        // For wishlist button state
        public bool IsInWishlist { get; set; }
    }

}
