using ITI_Raqmiya_MVC.Models;
using NuGet.Protocol;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IUserRepo
    {

        public List<User> GetAll();
        public User GetById(int id);
        public void Delete(User product);
        public void Update(User product);
        public void Add(User product);
        public void Save();

       

    }
}
