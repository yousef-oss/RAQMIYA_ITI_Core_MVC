using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITI_Raqmiya_MVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrder _orderRepo;
        private readonly IProductRepo _productRepo;

        public OrdersController(IOrder orderRepo, IProductRepo productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        // GET: /Orders/MyOrders (for logged-in buyers)
        //[Authorize]
        public IActionResult MyOrders()
        {
            //int buyerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int buyerId = 1;
            var orders = _orderRepo.GetByBuyerId(buyerId);
            return View(orders);
        }

        // GET: /Orders/GuestLookup?email=guest@example.com
        [AllowAnonymous]
        public IActionResult GuestLookup(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return View("EnterGuestEmail");

            var orders = _orderRepo.GetByEmail(email);
            return View("GuestOrders", orders);
        }

        // GET: /Orders/Details/5
        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = userIdClaim != null && int.TryParse(userIdClaim, out var uid) && order.BuyerId == uid;

            if (!isAdmin && !isOwner && order.GuestEmail != User.Identity.Name)
                return Unauthorized();

            return View(order);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);

            order.OrderedAt = DateTime.UtcNow;
            order.PaymentStatus = "completed"; // Simulate success
            _orderRepo.Add(order);
            _orderRepo.SaveChanges();

            // TODO: create license/access record here

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // GET: /Orders/Confirmation/5
        public IActionResult Confirmation(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: /Orders/Cancel/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null || order.PaymentStatus == "refunded")
                return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = userIdClaim != null && int.TryParse(userIdClaim, out var uid) && order.BuyerId == uid;

            if (!isAdmin && !isOwner)
                return Unauthorized();

            order.PaymentStatus = "refunded";
            _orderRepo.Update(order);
            _orderRepo.SaveChanges();

            // TODO: Revoke license/access

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Orders/Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var allOrders = _orderRepo.GetAll();
            return View(allOrders);
        }
    }
}
