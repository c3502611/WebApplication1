using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string query)
        {
            IQueryable<Product> products = _context.Products;

            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }

            var result = products.OrderByDescending(p => p.Id).ToList();
            return View(result);
        }

        public IActionResult Product(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            TempData.Keep("User");
            TempData.Keep("Role");

            return View(product);
        }

        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { query });
        }
    }
}
