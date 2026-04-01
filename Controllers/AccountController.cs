using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PracticeAspcoreMVC.Models;

namespace PracticeAspcoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private const string SessionKeyUsername = "LoggedInUser";
        private const string StaticUsername = "admin";
        private const string StaticPassword = "admin";

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString(SessionKeyUsername) != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Username == StaticUsername && model.Password == StaticPassword)
            {
                HttpContext.Session.SetString(SessionKeyUsername, model.Username);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
