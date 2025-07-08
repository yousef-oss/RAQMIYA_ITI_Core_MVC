using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using ITI_Raqmiya_MVC.ViewModels.Product;

namespace ITI_Raqmiya_MVC.Controllers
{

    //[Authorize]
        [Authorize] // Default to requiring authorization for the whole controller
        [Route("[controller]")] // Base route for the controller
        public class ProductsController : Controller
        {
            private readonly IProductRepository _productRepository;
            private readonly ICategoryRepository _categoryRepository; // Assuming you'll create this
            private readonly ITagRepository _tagRepository; // Assuming you'll create this
            private readonly ILogger<ProductsController> _logger;

            public ProductsController(
                IProductRepository productRepository,
                ICategoryRepository categoryRepository,
                ITagRepository tagRepository,
                ILogger<ProductsController> logger)
            {
                _productRepository = productRepository;
                _categoryRepository = categoryRepository;
                _tagRepository = tagRepository;
                _logger = logger;
            }

            // Helper to get current authenticated user's ID
            private int? GetCurrentUserId()
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
                return null;
            }

            // Helper to check if the current user is the product creator
            private async Task<bool> IsUserProductCreator(int productId)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return false;

                var product = await _productRepository.GetByIdAsync(productId);
                return product != null && product.CreatorId == userId.Value;
            }

            // --- Public Product Listings (AllowAnonymous) ---

            /// <summary>
            /// Displays a list of all published and public products.
            /// </summary>
            [HttpGet] // GET /Products
            [AllowAnonymous]
            public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
            {
                var products = await _productRepository.GetPublishedProductsAsync(pageNumber, pageSize);
                var totalProducts = await _productRepository.GetPublishedProductsCountAsync(); // You'll need to add this to IProductRepository
                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A",
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                        SalesCount = p.Orders.Count()
                    }).ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return View(viewModel); // Renders Views/Products/Index.cshtml
            }

            /// <summary>
            /// Displays details for a single product.
            /// Records a view for the product.
            /// </summary>
            [HttpGet("{id}")] // GET /Products/123
            [AllowAnonymous]
            public async Task<IActionResult> Details(int id)
            {
                var product = await _productRepository.GetProductWithAllDetailsAsync(id);

                if (product == null || product.Status != "published" || !product.IsPublic)
                {
                    // For public view, only show published and public products
                    return NotFound(); // Renders Views/Shared/NotFound.cshtml or default 404
                }

                // Record a view for this product
                var userId = GetCurrentUserId(); // Will be null if anonymous
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _productRepository.RecordProductViewAsync(id, userId, ipAddress);

                var viewModel = new ProductDetailViewModel
                {
                    Id = product.Id,
                    CreatorId = product.CreatorId,
                    CreatorUsername = product.Creator?.Username ?? "N/A",
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Currency = product.Currency,
                    ProductType = product.ProductType,
                    CoverImageUrl = product.CoverImageUrl,
                    PreviewVideoUrl = product.PreviewVideoUrl,
                    PublishedAt = product.PublishedAt,
                    Status = product.Status,
                    IsPublic = product.IsPublic,
                    Permalink = product.Permalink,
                    Files = product.Files.Select(f => new FileViewModel { Id = f.Id, Name = f.Name, FileUrl = f.FileUrl }).ToList(),
                    Variants = product.Variants.Select(v => new VariantViewModel { Id = v.Id, Name = v.Name, PriceAdjustment = v.PriceAdjustment }).ToList(),
                    OfferCodes = product.OfferCodes.Select(oc => new OfferCodeViewModel { Id = oc.Id, Code = oc.Code, DiscountValue = oc.DiscountValue }).ToList(),
                    Reviews = product.Reviews.Select(r => new ReviewViewModel { Id = r.Id, Rating = r.Rating, Comment = r.Comment, UserName = r.User?.Username ?? "Anonymous" }).ToList(),
                    Categories = product.ProductCategories.Select(pc => new CategoryViewModel { Id = pc.Category.Id, Name = pc.Category.Name }).ToList(),
                    Tags = product.ProductTags.Select(pt => new TagViewModel { Id = pt.Tag.Id, Name = pt.Tag.Name }).ToList(),
                    WishlistCount = product.WishlistItems.Count(),
                    AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                    SalesCount = product.Orders.Count(),
                    ViewsCount = product.ProductViews.Count(),
                    IsInWishlist = userId.HasValue && await _productRepository.IsProductInUserWishlistAsync(userId.Value, id)
                };

                return View(viewModel); // Renders Views/Products/Details.cshtml
            }

            // --- Creator-Specific Product Management ---

            /// <summary>
            /// Displays a list of products owned by the authenticated creator (including drafts).
            /// </summary>
            [HttpGet("MyProducts")] // GET /Products/MyProducts
            public async Task<IActionResult> MyProducts(int pageNumber = 1, int pageSize = 10)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return Forbid(); // Should be caught by [Authorize]

                var products = await _productRepository.GetProductsByCreatorIdAsync(userId.Value, pageNumber, pageSize);
                var totalProducts = await _productRepository.GetProductsByCreatorCountAsync(userId.Value); // Add this to IProductRepository
                var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A"
                    }).ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
                return View(viewModel); // Renders Views/Products/MyProducts.cshtml
            }

            /// <summary>
            /// Displays the form to create a new product.
            /// </summary>
            [HttpGet("Create")] // GET /Products/Create
            public async Task<IActionResult> Create()
            {
                var viewModel = new CreateProductViewModel();
                await PopulateCategoryAndTagLists(viewModel);
                return View(viewModel); // Renders Views/Products/Create.cshtml
            }

            /// <summary>
            /// Handles the submission of the new product form.
            /// </summary>
            [HttpPost("Create")] // POST /Products/Create
            [ValidateAntiForgeryToken] // Important for form security
            public async Task<IActionResult> Create([FromForm] CreateProductViewModel model)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return Forbid();

                if (!ModelState.IsValid)
                {
                    await PopulateCategoryAndTagLists(model); // Repopulate dropdowns on validation error
                    return View(model);
                }

                var newProduct = new Product
                {
                    CreatorId = userId.Value,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Currency = model.Currency,
                    ProductType = model.ProductType,
                    CoverImageUrl = model.CoverImageUrl,
                    PreviewVideoUrl = model.PreviewVideoUrl,
                    IsPublic = model.IsPublic,
                    Permalink = model.Permalink,
                    Status = "draft",
                    PublishedAt = model.IsPublic ? (DateTime?)DateTime.Now : null
                };

                try
                {
                    await _productRepository.AddAsync(newProduct);

                    // Add categories and tags
                    foreach (var catId in model.SelectedCategoryIds)
                    {
                        // You might want to validate if category exists here
                        newProduct.ProductCategories.Add(new ProductCategory { ProductId = newProduct.Id, CategoryId = catId });
                    }
                    foreach (var tagId in model.SelectedTagIds)
                    {
                        // You might want to validate if tag exists and is valid for categories here
                        newProduct.ProductTags.Add(new ProductTag { ProductId = newProduct.Id, TagId = tagId });
                    }
                    await _productRepository.UpdateAsync(newProduct); // Save categories/tags additions

                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToAction(nameof(Details), new { id = newProduct.Id });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating product: {Message}", ex.Message);
                    ModelState.AddModelError("", "Could not create product. A product with this permalink might already exist or other database error occurred.");
                    await PopulateCategoryAndTagLists(model);
                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error creating product: {Message}", ex.Message);
                    ModelState.AddModelError("", "An unexpected error occurred while creating the product.");
                    await PopulateCategoryAndTagLists(model);
                    return View(model);
                }
            }

            /// <summary>
            /// Displays the form to edit an existing product.
            /// </summary>
            [HttpGet("Edit/{id}")] // GET /Products/Edit/123
            public async Task<IActionResult> Edit(int id)
            {
                var product = await _productRepository.GetProductWithAllDetailsAsync(id); // Get all details for editing
                if (product == null) return NotFound();

                if (!await IsUserProductCreator(id)) return Forbid();

                var viewModel = new UpdateProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Currency = product.Currency,
                    ProductType = product.ProductType,
                    CoverImageUrl = product.CoverImageUrl,
                    PreviewVideoUrl = product.PreviewVideoUrl,
                    Status = product.Status,
                    IsPublic = product.IsPublic,
                    Permalink = product.Permalink,
                    SelectedCategoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
                    SelectedTagIds = product.ProductTags.Select(pt => pt.TagId).ToList()
                };

                await PopulateCategoryAndTagLists(viewModel);
                return View(viewModel); // Renders Views/Products/Edit.cshtml
            }

            /// <summary>
            /// Handles the submission of the product edit form.
            /// </summary>
            [HttpPost("Edit/{id}")] // POST /Products/Edit/123
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [FromForm] UpdateProductViewModel model)
            {
                if (id != model.Id) return BadRequest();

                var product = await _productRepository.GetProductWithAllDetailsAsync(id); // Get with details to update collections
                if (product == null) return NotFound();

                if (!await IsUserProductCreator(id)) return Forbid();

                if (!ModelState.IsValid)
                {
                    await PopulateCategoryAndTagLists(model);
                    return View(model);
                }

                // Update scalar properties
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Currency = model.Currency;
                product.ProductType = model.ProductType;
                product.CoverImageUrl = model.CoverImageUrl;
                product.PreviewVideoUrl = model.PreviewVideoUrl;
                product.Permalink = model.Permalink;

                if (product.IsPublic != model.IsPublic) // Check if public status changed
                {
                    product.IsPublic = model.IsPublic;
                    if (product.IsPublic && !product.PublishedAt.HasValue)
                    {
                        product.PublishedAt = DateTime.Now; // Use DateTime.UtcNow
                    }
                }
                product.Status = model.Status;

                try
                {
                    // Update Categories (simple replacement logic)
                    var existingCategoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
                    var categoriesToAdd = model.SelectedCategoryIds.Except(existingCategoryIds).ToList();
                    var categoriesToRemove = existingCategoryIds.Except(model.SelectedCategoryIds).ToList();

                    foreach (var catId in categoriesToRemove)
                    {
                        var pcToRemove = product.ProductCategories.FirstOrDefault(pc => pc.CategoryId == catId);
                        if (pcToRemove != null) product.ProductCategories.Remove(pcToRemove);
                    }
                    foreach (var catId in categoriesToAdd)
                    {
                        // Validate category exists
                        product.ProductCategories.Add(new ProductCategory { ProductId = product.Id, CategoryId = catId });
                    }

                    // Update Tags (simple replacement logic)
                    var existingTagIds = product.ProductTags.Select(pt => pt.TagId).ToList();
                    var tagsToAdd = model.SelectedTagIds.Except(existingTagIds).ToList();
                    var tagsToRemove = existingTagIds.Except(model.SelectedTagIds).ToList();

                    foreach (var tagId in tagsToRemove)
                    {
                        var ptToRemove = product.ProductTags.FirstOrDefault(pt => pt.TagId == tagId);
                        if (ptToRemove != null) product.ProductTags.Remove(ptToRemove);
                    }
                    foreach (var tagId in tagsToAdd)
                    {
                        // Validate tag exists and is valid for product's categories
                        product.ProductTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                    }

                    await _productRepository.UpdateAsync(product); // This will save changes to collections too

                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = product.Id });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error updating product {ProductId}: {Message}", id, ex.Message);
                    ModelState.AddModelError("", "Could not update product. Check if permalink is unique or other data is valid.");
                    await PopulateCategoryAndTagLists(model);
                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error updating product {ProductId}: {Message}", id, ex.Message);
                    ModelState.AddModelError("", "An unexpected error occurred while updating the product.");
                    await PopulateCategoryAndTagLists(model);
                    return View(model);
                }
            }

            /// <summary>
            /// Displays confirmation for deleting a product.
            /// </summary>
            [HttpGet("Delete/{id}")] // GET /Products/Delete/123
            public async Task<IActionResult> Delete(int id)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null) return NotFound();

                if (!await IsUserProductCreator(id)) return Forbid();

                return View(product); // Renders Views/Products/Delete.cshtml (simple confirmation)
            }

            /// <summary>
            /// Handles the deletion of a product.
            /// </summary>
            [HttpPost("Delete/{id}")] // POST /Products/Delete/123
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null) return NotFound(); // Already deleted or not found

                if (!await IsUserProductCreator(id)) return Forbid();

                try
                {
                    await _productRepository.DeleteAsync(id);
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                    return RedirectToAction(nameof(MyProducts));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting product {ProductId}: {Message}", id, ex.Message);
                    ModelState.AddModelError("", "An error occurred while deleting the product. It might have related records (e.g., active subscriptions) preventing deletion.");
                    return View("Delete", product); // Return to delete confirmation with error
                }
            }

            // --- Wishlist Actions ---

            /// <summary>
            /// Adds a product to the authenticated user's wishlist.
            /// </summary>
            [HttpPost("{productId}/AddToWishlist")] // POST /Products/123/AddToWishlist
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddToWishlist(int productId)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return Unauthorized(); // Should be caught by [Authorize]

                if (!await _productRepository.ExistsAsync(productId)) return NotFound("Product not found.");

                try
                {
                    await _productRepository.AddProductToWishlistAsync(userId.Value, productId);
                    TempData["SuccessMessage"] = "Product added to wishlist!";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Product {ProductId} already in user {UserId}'s wishlist.", productId, userId);
                    TempData["ErrorMessage"] = "Product is already in your wishlist.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding product {ProductId} to user {UserId}'s wishlist: {Message}", productId, userId, ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while adding to wishlist.";
                }

                return RedirectToAction(nameof(Details), new { id = productId });
            }

            /// <summary>
            /// Removes a product from the authenticated user's wishlist.
            /// </summary>
            [HttpPost("{productId}/RemoveFromWishlist")] // POST /Products/123/RemoveFromWishlist
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> RemoveFromWishlist(int productId)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return Unauthorized();

                if (!await _productRepository.ExistsAsync(productId)) return NotFound("Product not found.");

                try
                {
                    await _productRepository.RemoveProductFromWishlistAsync(userId.Value, productId);
                    TempData["SuccessMessage"] = "Product removed from wishlist.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing product {ProductId} from user {UserId}'s wishlist: {Message}", productId, userId, ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while removing from wishlist.";
                }

                return RedirectToAction(nameof(Details), new { id = productId });
            }

            /// <summary>
            /// Displays the authenticated user's wishlist.
            /// </summary>
            [HttpGet("MyWishlist")] // GET /Products/MyWishlist
            public async Task<IActionResult> MyWishlist(int pageNumber = 1, int pageSize = 10)
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue) return Forbid();

                var products = await _productRepository.GetUserWishlistAsync(userId.Value, pageNumber, pageSize);
                // You'd need a GetUserWishlistCountAsync for total pages. For now, assume simple list.

                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A"
                    }).ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 1 // Placeholder
                };
                return View(viewModel); // Renders Views/Products/MyWishlist.cshtml
            }


            // --- Derived Metrics Views (AllowAnonymous) ---

            /// <summary>
            /// Displays a list of most wished products.
            /// </summary>
            [HttpGet("MostWished")]
            [AllowAnonymous]
            public async Task<IActionResult> MostWished(int count = 10)
            {
                var products = await _productRepository.GetMostWishedProductsAsync(count);
                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A",
                        SalesCount = p.Orders.Count(),
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                    }).ToList()
                };
                return View("Index", viewModel); // Can reuse the Index view for generic lists
            }

            /// <summary>
            /// Displays a list of top-rated products.
            /// </summary>
            [HttpGet("TopRated")]
            [AllowAnonymous]
            public async Task<IActionResult> TopRated(int count = 10)
            {
                var products = await _productRepository.GetTopRatedProductsAsync(count);
                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A",
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                        SalesCount = p.Orders.Count()
                    }).ToList()
                };
                return View("Index", viewModel);
            }

            /// <summary>
            /// Displays a list of best-selling products.
            /// </summary>
            [HttpGet("BestSelling")]
            [AllowAnonymous]
            public async Task<IActionResult> BestSelling(int count = 10)
            {
                var products = await _productRepository.GetBestSellingProductsAsync(count);
                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A",
                        SalesCount = p.Orders.Count(),
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                    }).ToList()
                };
                return View("Index", viewModel);
            }

            /// <summary>
            /// Displays a list of trendy products.
            /// </summary>
            [HttpGet("Trendy")]
            [AllowAnonymous]
            public async Task<IActionResult> Trendy(int count = 10, int daysBack = 30)
            {
                var products = await _productRepository.GetTrendyProductsAsync(count, daysBack);
                var viewModel = new ProductListViewModel
                {
                    Products = products.Select(p => new ProductListItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Permalink = p.Permalink,
                        Price = p.Price,
                        Currency = p.Currency,
                        CoverImageUrl = p.CoverImageUrl,
                        CreatorUsername = p.Creator?.Username ?? "N/A",
                        SalesCount = p.Orders.Count(),
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
                    }).ToList()
                };
                return View("Index", viewModel);
            }

            // --- Helper Methods ---

            private async Task PopulateCategoryAndTagLists(CreateProductViewModel model)
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync(); // Assuming this method exists
                model.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = model.SelectedCategoryIds.Contains(c.Id)
                }).ToList();

                // For tags, you might want to initially load all tags or common tags.
                // The logic for "available tags for product's categories" is more complex
                // and would typically be handled via client-side JS after category selection.
                // For a server-side MVC form, you might just load all tags or a predefined set.
                var allTags = await _tagRepository.GetAllTagsAsync(); // Assuming this method exists
                model.AvailableTags = allTags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name,
                    Selected = model.SelectedTagIds.Contains(t.Id)
                }).ToList();
            }

            private async Task PopulateCategoryAndTagLists(UpdateProductViewModel model)
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                model.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = model.SelectedCategoryIds.Contains(c.Id)
                }).ToList();

                var allTags = await _tagRepository.GetAllTagsAsync();
                model.AvailableTags = allTags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name,
                    Selected = model.SelectedTagIds.Contains(t.Id)
                }).ToList();
            }
        }
    }
