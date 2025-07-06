using ITI_Raqmiya_MVC.Models;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IOrder
    {
        public List<Order> GetAll();
        public Order GetById(int id);
        public void Delete(Order order);
        public void Update(Order order);
        public void Add(Order order);
        public void Save();
    }
}
