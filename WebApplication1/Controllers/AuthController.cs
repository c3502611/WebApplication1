using Microsoft.AspNetCore.Mvc;

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
            // Mock login check
            if (username == "admin" && password == "password")
            {
                TempData["User"] = "Admin";
                TempData["Role"] = "Admin";
                return RedirectToAction("Index", "Home");
            }
            else if (username == "customer" && password == "password")
            {
                TempData["User"] = "Customer";
                TempData["Cart"] = "Customer";
                return RedirectToAction("Index", "Home");
            }

                ViewBag.Error = "Invalid username or password.";
            return View();
        }

        public IActionResult Logout()
        {
            TempData.Remove("User");
            return RedirectToAction("Index", "Home");
        }
    }
}
