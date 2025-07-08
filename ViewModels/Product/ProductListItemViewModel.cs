﻿namespace ITI_Raqmiya_MVC.ViewModels.Product
{
    public class ProductListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Permalink { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string CreatorUsername { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int SalesCount { get; set; }
    }
}
