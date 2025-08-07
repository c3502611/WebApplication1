using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // <-- for Include()
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            IQueryable<Product> products = _context.Products.Include(p => p.Images); // <-- Include Images

            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }

            var result = products.OrderByDescending(p => p.Id).ToList();
            return View(result);
        }

        public IActionResult Product(int id)
        {
            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            TempData.Keep("User");
            TempData.Keep("Role");

            var currentTags = product.Tags?.ToLower()
    .       Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? Array.Empty<string>();

            var recommended = _context.Products
                .Include(p => p.Images)
                .Where(p => p.Id != id &&
                            (
                                p.CategoryId == product.CategoryId ||
                                currentTags.Any(tag => p.Tags.ToLower().Contains(tag))
                            ))
                .Distinct()
                .Take(4)
                .ToList();

            if (recommended == null || !recommended.Any())
            {
                recommended = _context.Products
                    .Where(p => p.Id != id)
                    .Include(p => p.Images)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(4)
                    .ToList();
            }

            ViewBag.RecommendedProducts = recommended;
            return View(product);
        }



        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SearchResults", new List<Product>());
            }

            var terms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var results = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p =>
                    terms.All(term =>
                        (p.Name != null && p.Name.ToLower().Contains(term)) ||
                        (p.Tags != null && p.Tags.ToLower().Contains(term)) ||
                        (p.Category != null && p.Category.Name.ToLower().Contains(term))
                    )
                )
                .ToList();

            return View("SearchResults", results);
        }
    }
}