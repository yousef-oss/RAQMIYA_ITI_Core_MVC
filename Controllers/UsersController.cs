using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repos_Implementation;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITI_Raqmiya_MVC.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserRepo _userRepo;

        public UsersController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: Users (Admin only)
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var users = _userRepo.GetAll();
            return View(users);
        }

        // GET: Users/Details/5 (Owner or Admin)
        public IActionResult Details(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null)
                return NotFound();  // Core version of 404

            var currentUserId = int.Parse(User.Identity.Name); // Or retrieve from claims
            if (!User.IsInRole("Admin") && user.Id != currentUserId)
                return Unauthorized(); // Core version of 401

            return View(user);
        }

        // GET: Users/Edit/5 (Owner only)
        public ActionResult Edit(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null)
                return NotFound();

            var currentUserId = int.Parse(User.Identity.Name);
            if (user.Id != currentUserId)
                return Unauthorized(); 


            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (user.Id != currentUserId)
                return Unauthorized();


            if (!ModelState.IsValid)
                return View(user);

            _userRepo.Update(user);
            return RedirectToAction("Details", new { id = user.Id });
        }

        // GET: Users/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _userRepo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
