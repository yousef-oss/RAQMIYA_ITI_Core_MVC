using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Models;
using ITI_Raqmiya_MVC.Repository.Repos_Implementation;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITI_Raqmiya_MVC.Controllers
{
    public class UsersController : Controller
    {

        

        private readonly IUserRepo userRepo;

        public UsersController(IUserRepo _userRepo)
        {
            userRepo = _userRepo;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await userRepo.GetAllAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await userRepo.GetByIdAsync(id.Value);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,PasswordHash,Username,CreatedAt,LastLogin,IsCreator,ProfileDescription,ProfileImageUrl,StripeConnectAccountId,PayoutSettings")] User user)
        {
            if (ModelState.IsValid)
            {
                await userRepo.AddAsync(user);
                await userRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await userRepo.GetByIdAsync(id.Value);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,PasswordHash,Username,CreatedAt,LastLogin,IsCreator,ProfileDescription,ProfileImageUrl,StripeConnectAccountId,PayoutSettings")] User user)
        {
            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await userRepo.UpdateAsync(user);
                    await userRepo.SaveAsync();
                }
                catch
                {
                    var exists = await userRepo.GetByIdAsync(user.Id);
                    if (exists == null)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await userRepo.GetByIdAsync(id.Value);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await userRepo.GetByIdAsync(id);
            if (user != null)
            {
                await userRepo.DeleteAsync(user);
                await userRepo.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
