using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.EntityFrameworkCore;

namespace ITI_Raqmiya_MVC.Repository.Repos_Implementation
{
    public class UserRepo : IUserRepo
    {
        private readonly RaqmiyaContext _context;

        public UserRepo(RaqmiyaContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get all users without navigation properties (lightweight)
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Get all users including related navigation properties
        public async Task<List<User>> GetAllWithDetailsAsync()
        {
            return await _context.Users
                .Include(u => u.Products)
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .Include(u => u.Subscriptions)
                .Include(u => u.Posts)
                .ToListAsync();
        }

        // Get single user by ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Get single user by ID with relations
        public async Task<User?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Products)
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .Include(u => u.Subscriptions)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Add new user
        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _context.Users.AddAsync(user);
        }

        // Update user
        public Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        // Delete user
        public Task DeleteAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        // Save changes to database
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
