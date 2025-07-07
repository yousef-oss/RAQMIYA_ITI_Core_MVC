using ITI_Raqmiya_MVC.Models;
using NuGet.Protocol;

namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IUserRepo
    {

        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user); // Optional — EF tracks entity changes
        Task DeleteAsync(User user);
        Task SaveAsync();



    }
}
