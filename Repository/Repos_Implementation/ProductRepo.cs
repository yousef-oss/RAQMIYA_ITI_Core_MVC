using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using System;

namespace ITI_Raqmiya_MVC.Repository.Repos_Implementation
{
    public class ProductRepo : IProductRepo
    {
        private readonly RaqmiyaContext _context;

        public ProductRepo(RaqmiyaContext context)
        {
            _context = context;
        }

        public Product GetById(int id) => _context.Products.Find(id);

        public IEnumerable<Product> GetAll() => _context.Products.ToList();

        public IEnumerable<Product> GetAllPublished() =>
            _context.Products.Where(p => p.IsPublic && p.Status == "published").ToList();

        public IEnumerable<Product> GetAllByCreatorId(int creatorId) =>
            _context.Products.Where(p => p.CreatorId == creatorId).ToList();

        public void Add(Product product) => _context.Products.Add(product);

        public void Update(Product product) => _context.Products.Update(product);

        public void Delete(int id)
        {
            var product = GetById(id);
            if (product != null)
                _context.Products.Remove(product);
        }

        public void SaveChanges() => _context.SaveChanges();

    }
}
