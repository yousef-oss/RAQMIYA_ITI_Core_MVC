using ITI_Raqmiya_MVC.ViewModels;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;

using Microsoft.AspNetCore.Mvc;

namespace ITI_Raqmiya_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepo;

        public AccountController(IAuthRepository authService)
        {
            _authRepo = authService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _authRepo.RegisterAsync(model.Username, model.Password))
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

            if (await _authRepo.LoginAsync(model.Username, model.Password))
            {
                HttpContext.Session.SetString("Username", model.Username);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid credentials.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}
