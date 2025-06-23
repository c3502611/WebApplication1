using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            var products = MockData.GetProducts();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = MockData.GetProducts().FirstOrDefault(p => p.Id == id);
            return product == null ? NotFound() : View(product);
        }
    }
}
