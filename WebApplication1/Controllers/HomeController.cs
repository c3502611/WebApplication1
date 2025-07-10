using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string query)
        {
            var products = MockData.GetProducts();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();
                products = products
                    .Where(p => p.Name.ToLower().Contains(query) || p.Description.ToLower().Contains(query))
                    .ToList();
            }

            return View(products);
        }

        public IActionResult Product(int id)
        {
            var product = MockData.GetProducts().FirstOrDefault(p => p.Id == id);
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
