using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("User", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var hasher = new PasswordHasher<User>();
            var newUser = new User
            {
                Username = username,
                Email = email,
                Role = "Customer"
            };
            newUser.Password = hasher.HashPassword(newUser, password); // hashed here

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["RegisterSuccess"] = $"Welcome {username}! Your account has been created.";
            return RedirectToAction("Index", "Home");
        }
    }
}
