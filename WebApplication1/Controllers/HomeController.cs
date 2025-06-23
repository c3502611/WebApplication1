using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Graphic Tee",
                    Price = 25.99m,
                    Description = "Bold, comfortable cotton shirt.",
                    Category = "Shirts",
                    ImageUrl = "https://via.placeholder.com/300x300?text=Graphic+Tee",
                    StockQuantity = 15
                },
                new Product
                {
                    Id = 2,
                    Name = "Denim Jacket",
                    Price = 59.99m,
                    Description = "Stylish and timeless outerwear.",
                    Category = "Jackets",
                    ImageUrl = "https://via.placeholder.com/300x300?text=Denim+Jacket",
                    StockQuantity = 8
                }
            };

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
            var products = MockData.GetProducts();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();
                products = products
                    .Where(p => p.Name.ToLower().Contains(query) || p.Description.ToLower().Contains(query))
                    .ToList();
            }

            return View("Index", products); // reuse your main product list view
        }

    }
}
