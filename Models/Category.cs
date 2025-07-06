namespace ITI_Raqmiya_MVC.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary Key (int)
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; } // Foreign Key to itself (int?)

        // Navigation properties for hierarchical structure
        public Category? ParentCategory { get; set; }
        public ICollection<Category> Subcategories { get; set; } = new List<Category>();

        // Navigation property for many-to-many with Product
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }

}
