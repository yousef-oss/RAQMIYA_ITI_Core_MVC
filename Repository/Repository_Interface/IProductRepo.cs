using ITI_Raqmiya_MVC.Models;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IProductRepo
    {
        public List<Product> GetAll();
        public Product GetById(int id);
        public void Delete(Product product);
        public void Update(Product product);
        public void Add(Product product);
        public void Save();


    }
}
