using ITI_Raqmiya_MVC.Models;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IProductRepo
    {
        public List<Product> GetAll();
        public Product GetById(int id);
        public void Delete(Product user);
        public void Update(Product user);
        public void Add(Product user);
        public void Save();


    }
}
