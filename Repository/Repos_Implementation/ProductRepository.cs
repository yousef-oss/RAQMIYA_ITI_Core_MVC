﻿using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace ITI_Raqmiya_MVC.Repository.Repos_Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly RaqmiyaContext _context;

        public ProductRepository(RaqmiyaContext context)
        {
            _context = context;
        }

        // --- Basic CRUD Operations ---
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        // --- Get Products with Related Data ---
        public async Task<Product?> GetProductWithAllDetailsAsync(int productId)
        {
            return await _context.Products
                .AsNoTracking() // Good for read-only operations
                .Include(p => p.Creator)
                .Include(p => p.Files)
                .Include(p => p.Variants)
                .Include(p => p.OfferCodes)
                .Include(p => p.Reviews)
                .Include(p => p.Subscriptions)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category) // Include the actual Category object
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag) // Include the actual Tag object
                .Include(p => p.WishlistItems) // Include for derived metrics or checking if in wishlist
                .Include(p => p.ProductViews) // Include for derived metrics
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<List<Product>> GetProductsWithCreatorAndFilesAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Include(p => p.Files)
                .ToListAsync();
        }

        // --- Filtering and Searching ---
        private IQueryable<Product> ApplyPagination(IQueryable<Product> query, int? pageNumber, int? pageSize)
        {
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                return query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Creator) // Include common details
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId));

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByTagIdAsync(int tagId, int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.Creator)
                .Where(p => p.ProductTags.Any(pt => pt.TagId == tagId));

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm, int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCreatorIdAsync(int creatorId, int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Where(p => p.CreatorId == creatorId);

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }

        public async Task<List<Product>> GetPublishedProductsAsync(int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Include(p => p.Files) // Often useful to show files on product listings
                .Where(p => p.Status == "published" && p.IsPublic);

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }


        // --- Tag Management for Products ---
        public async Task AddProductTagAsync(int productId, int tagId)
        {
            if (!await ProductTagExistsAsync(productId, tagId))
            {
                var productTag = new ProductTag { ProductId = productId, TagId = tagId };
                await _context.ProductTags.AddAsync(productTag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductTagAsync(int productId, int tagId)
        {
            var productTag = await _context.ProductTags
                .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
            if (productTag != null)
            {
                _context.ProductTags.Remove(productTag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductTagExistsAsync(int productId, int tagId)
        {
            return await _context.ProductTags.AnyAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
        }

        public async Task<List<Tag>> GetTagsForProductAsync(int productId)
        {
            return await _context.ProductTags
                .AsNoTracking()
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.Tag)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetAvailableTagsForProductCategoriesAsync(int productId)
        {
            // Get the categories associated with the product
            var categoryIds = await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            if (!categoryIds.Any())
            {
                return new List<Tag>();
            }

            // Get tags associated with those categories
            return await _context.CategoryTags
                .AsNoTracking()
                .Where(ct => categoryIds.Contains(ct.CategoryId))
                .Select(ct => ct.Tag)
                .Distinct() // Ensure unique tags
                .ToListAsync();
        }

        // --- Wishlist Operations ---
        public async Task AddProductToWishlistAsync(int userId, int productId)
        {
            if (!await IsProductInUserWishlistAsync(userId, productId))
            {
                var wishlistItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedAt = DateTime.Now // Use DateTime.UtcNow for production
                };
                await _context.WishlistItems.AddAsync(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromWishlistAsync(int userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.UserId == userId && wi.ProductId == productId);
            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsProductInUserWishlistAsync(int userId, int productId)
        {
            return await _context.WishlistItems.AnyAsync(wi => wi.UserId == userId && wi.ProductId == productId);
        }

        public async Task<List<Product>> GetUserWishlistAsync(int userId, int? pageNumber = 1, int? pageSize = 10)
        {
            var query = _context.WishlistItems
                .AsNoTracking()
                .Where(wi => wi.UserId == userId)
                .Select(wi => wi.Product)
                .Include(p => p.Creator) // Include some product details
                .Include(p => p.Files); // Include some product details

            return await ApplyPagination(query, pageNumber, pageSize).ToListAsync();
        }

        // --- Product View Tracking ---
        public async Task RecordProductViewAsync(int productId, int? userId, string? ipAddress)
        {
            var productView = new ProductView
            {
                ProductId = productId,
                UserId = userId,
                ViewedAt = DateTime.Now, // Use DateTime.UtcNow for production
                IpAddress = ipAddress
            };
            await _context.ProductViews.AddAsync(productView);
            await _context.SaveChangesAsync();
        }

        // --- Derived Metrics Queries ---

        public async Task<List<Product>> GetMostWishedProductsAsync(int count = 10)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Creator) // Include relevant details
                .OrderByDescending(p => p.WishlistItems.Count())
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetTopRatedProductsAsync(int count = 10)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Where(p => p.Reviews.Any()) // Only products with reviews
                .OrderByDescending(p => p.Reviews.Average(r => r.Rating))
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetBestSellingProductsAsync(int count = 10)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .OrderByDescending(p => p.Orders.Count()) // Assuming one order item per product
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetTrendyProductsAsync(int count = 10, int daysBack = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysBack); // Use DateTime.UtcNow for production

            // This is a simplified "trendy" calculation.
            // A more sophisticated algorithm would involve weighting different factors,
            // normalization, and potentially a background service to pre-calculate scores.
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Creator)
                .Select(p => new
                {
                    Product = p,
                    RecentSales = p.Orders.Count(o => o.OrderedAt >= cutoffDate),
                    RecentWishlistAdds = p.WishlistItems.Count(wi => wi.AddedAt >= cutoffDate),
                    RecentViews = p.ProductViews.Count(pv => pv.ViewedAt >= cutoffDate),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0 // Include rating as a factor
                })
                // Order by a combination of factors. Adjust weights as needed.
                .OrderByDescending(x => x.RecentSales * 3 // Sales carry more weight
                                     + x.RecentWishlistAdds * 2
                                     + x.RecentViews
                                     + x.AverageRating * 10 // High ratings might signify trend
                                     )
                .Select(x => x.Product)
                .Take(count)
                .ToListAsync();
        }

        public async Task<double> GetPublishedProductsCountAsync()
        {
            return await _context.Products.CountAsync(p => p.Status == "published" && p.IsPublic);
        }

        public async Task<double> GetProductsByCreatorCountAsync(int creatorId)
        {
            return await _context.Products.CountAsync(p => p.CreatorId == creatorId);
        }

        // Optional: If you need pagination for the user's wishlist
        public async Task<double> GetUserWishlistCountAsync(int userId)
        {
            return await _context.WishlistItems.CountAsync(wi => wi.UserId == userId);
        }
    }
}
