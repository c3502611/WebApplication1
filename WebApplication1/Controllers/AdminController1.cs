using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;      
using Microsoft.EntityFrameworkCore; 

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (TempData["Role"]?.ToString() != "Admin")
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var productCount = _context.Products.Count();

            ViewBag.ProductCount = productCount;

            return View();
        }
    }
}
