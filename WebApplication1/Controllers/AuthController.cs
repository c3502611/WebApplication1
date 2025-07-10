using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                TempData["User"] = "Admin";
                TempData["Role"] = "Admin";
                return RedirectToAction("Index", "Home");
            }
            else if (username == "customer" && password == "password")
            {
                TempData["User"] = "Customer";
                TempData["Role"] = "Customer";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        public IActionResult Logout()
        {
            TempData.Remove("User");
            TempData.Remove("Role");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string email, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            TempData["User"] = username;
            TempData["Role"] = "Customer";
            TempData["RegisterSuccess"] = $"User {username} registered successfully with email {email}.";

            return RedirectToAction("Index", "Home"); 
        }
    }
}

