namespace ITI_Raqmiya_MVC.ViewModels.Product
{
    public class ProductListViewModel
    {
        public List<ProductListItemViewModel> Products { get; set; } = new List<ProductListItemViewModel>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }




}
