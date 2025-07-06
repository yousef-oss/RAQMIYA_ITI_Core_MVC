using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;

namespace ITI_Raqmiya_MVC.Controllers
{

    //[Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductRepo _productRepo;

        public ProductsController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        // GET: /Products
        //[AllowAnonymous]
        public IActionResult Index()
        {
            var products = _productRepo.GetAllPublished();
            return View(products);
        }

        // GET: /Products/MyProducts
        public IActionResult MyProducts()
        {
            int creatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var myProducts = _productRepo.GetAllByCreatorId(creatorId);
            return View(myProducts);
        }

        // GET: /Products/Details/5
        //[AllowAnonymous]
        public IActionResult Details(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null || (!product.IsPublic && !User.IsInRole("Admin") && product.CreatorId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                return NotFound();

            return View(product);
        }

        // GET: /Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Products/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View();

            product.CreatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            product.PublishedAt = DateTime.UtcNow;
            product.Status = "draft";
            _productRepo.Add(product);
            _productRepo.SaveChanges();

            return RedirectToAction(nameof(MyProducts));
        }

        // GET: /Products/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null || product.CreatorId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Unauthorized();

            return View(product);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            var original = _productRepo.GetById(product.Id);
            if (original == null || original.CreatorId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Unauthorized();

            // Only update editable fields
            original.Name = product.Name;
            original.Description = product.Description;
            original.Price = product.Price;
            original.Currency = product.Currency;
            original.ProductType = product.ProductType;
            original.CoverImageUrl = product.CoverImageUrl;
            original.PreviewVideoUrl = product.PreviewVideoUrl;
            original.IsPublic = product.IsPublic;
            original.Status = product.Status;
            original.Permalink = product.Permalink;

            _productRepo.Update(original);
            _productRepo.SaveChanges();

            return RedirectToAction(nameof(MyProducts));
        }

        // GET: /Products/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null || product.CreatorId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Unauthorized();

            return View(product);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null || product.CreatorId.ToString() != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Unauthorized();

            _productRepo.Delete(id);
            _productRepo.SaveChanges();
            return RedirectToAction(nameof(MyProducts));
        }
    }

}
