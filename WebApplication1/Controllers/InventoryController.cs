using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class InventoryController : Controller
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Admin");

            TempData.Keep("User");
            TempData.Keep("Role");

            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Product product, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                ImageFile.CopyTo(ms);
                product.ImageData = ms.ToArray();
                product.ImageMimeType = ImageFile.ContentType;
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

           
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await ImageFile.CopyToAsync(memoryStream);
                existingProduct.ImageData = memoryStream.ToArray();
                existingProduct.ImageMimeType = ImageFile.ContentType;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult GetImage(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null || product.ImageData == null || string.IsNullOrEmpty(product.ImageMimeType))
            {
                return NotFound();
            }

            return File(product.ImageData, product.ImageMimeType);
        }
    }
}
