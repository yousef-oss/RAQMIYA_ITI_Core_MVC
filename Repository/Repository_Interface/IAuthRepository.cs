namespace ITI_Raqmiya_MVC.Repository.Repository_Interface
{
    public interface IAuthRepository
    {
        Task<bool> RegisterAsync(string username, string password);
        Task<bool> LoginAsync(string username, string password);
    }
}
