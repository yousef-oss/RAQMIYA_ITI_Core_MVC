using ITI_Raqmiya_MVC.Models;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IProductRepo
    {
        Product GetById(int id);
        IEnumerable<Product> GetAllPublished();
        IEnumerable<Product> GetAllByCreatorId(int creatorId);
        IEnumerable<Product> GetAll(); // Admin

        IEnumerable<Product> GetAllFeatured();

        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        void SaveChanges();


    }
}
