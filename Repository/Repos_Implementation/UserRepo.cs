﻿using ITI_Raqmiya_MVC.Data;
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

        public User GetById(int id) => _context.Users.Find(id);

        public IEnumerable<User> GetAll() => _context.Users.ToList();

        public void Add(User user) => _context.Users.Add(user);

        public void Update(User user) => _context.Users.Update(user);

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
                _context.Users.Remove(user);
        }

        public void SaveChanges() => _context.SaveChanges();

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
    }
}
