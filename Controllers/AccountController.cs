using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using ITI_Raqmiya_MVC.ViewModels;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITI_Raqmiya_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly RaqmiyaContext _db;

        public AccountController(IAuthRepository authService, RaqmiyaContext db)
        {
            _authRepo = authService;
            _db = db;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _authRepo.RegisterAsync(model.Email, model.Username, model.Password, model.Role))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password.");
                return View(model);
            }

            var inputHash = _authRepo.HashPassword(model.Password, user.Salt);
            if (inputHash != user.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid Username or Password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsCreator ? "Creator" : "User"),
                new Claim("LastLogin", DateTime.UtcNow.ToString("o"))

            };



            //DateTime.Parse(User.FindFirst("LastLogin")?.Value); if you want to access the last login later.
            user.LastLogin = DateTime.UtcNow;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();


            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);

            // add session
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            //await HttpContext.SignInAsync(principal);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session data completely
            HttpContext.Session.Clear();

            // Expire all cookies manually (optional but safe)
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Login", "Account");
        }


    }

}
