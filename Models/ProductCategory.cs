namespace ITI_Raqmiya_MVC.Models
{
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        public Product Product { get; set; } = null!;
        public Category Category { get; set; } = null!; 
    }

}
