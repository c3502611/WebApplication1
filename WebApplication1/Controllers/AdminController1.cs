using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            var products = MockData.GetProducts();
            var orders = MockData.GetOrders();

            ViewBag.ProductCount = products.Count;
            ViewBag.OrderCount = orders.Count;

            return View();
        }
    }
}
