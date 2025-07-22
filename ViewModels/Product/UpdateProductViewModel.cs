using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ITI_Raqmiya_MVC.ViewModels.Product
{
    public class UpdateProductViewModel
    {
        public int Id { get; set; }

        [Required] // Name is required for update
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000.00, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "USD";

        [Required]
        [StringLength(50)]
        public string ProductType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        [StringLength(500)]
        public string? PreviewVideoUrl { get; set; }

        [Required]
        public string Status { get; set; } = "draft"; // e.g., "draft", "published", "archived", "unlisted"

        public bool IsPublic { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Permalink must be lowercase alphanumeric with hyphens.")]
        public string Permalink { get; set; } = string.Empty;

        // For dropdowns/multi-select in the view
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public List<SelectListItem> AvailableTags { get; set; } = new List<SelectListItem>();
    }
}
