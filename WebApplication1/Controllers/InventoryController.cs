using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WebApplication1.Controllers
{
    public class InventoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public InventoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Inventory
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Admin");

            TempData.Keep("User");
            TempData.Keep("Role");

            var products = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .ToList();

            return View(products);
        }

        // GET: /Inventory/Add
        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel model, List<IFormFile> ImageFiles)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Admin");

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Tags = model.Tags,
                    Images = new List<ItemImage>()
                };

                var uploadFolder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadFolder);

                if (ImageFiles != null && ImageFiles.Any())
                {
                    foreach (var imageFile in ImageFiles)
                    {
                        var uniqueFileName = $"product_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var imagePath = Path.Combine("images/products", uniqueFileName).Replace("\\", "/");
                        var fullFilePath = Path.Combine(_env.WebRootPath, imagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                        // Copy file to memory first
                        byte[] imageData;
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            imageData = ms.ToArray();
                        }

                        // Save to disk
                        await System.IO.File.WriteAllBytesAsync(fullFilePath, imageData);

                        product.Images.Add(new ItemImage
                        {
                            ImageMimeType = imageFile.ContentType,
                            ImagePath = imagePath,
                            ImageData = imageData
                        });
                    }
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            model.Categories = _context.Categories.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var viewModel = new ProductViewModel
            {
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel model, List<IFormFile> ImageFiles)
        {
            if (id != model.Id)
                return BadRequest();

            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update product fields
                product.Name = model.Name;
                product.Description = model.Description;
                product.CategoryId = model.CategoryId;
                product.Price = model.Price;
                product.StockQuantity = model.StockQuantity;
                product.Tags = model.Tags;

                // Remove selected images
                if (model.ImagesToRemove != null && model.ImagesToRemove.Any())
                {
                    var imagesToDelete = product.Images
                        .Where(img => model.ImagesToRemove.Contains(img.Id))
                        .ToList();

                    foreach (var image in imagesToDelete)
                    {
                        product.Images.Remove(image);
                        _context.ItemImages.Remove(image);

                        if (!string.IsNullOrEmpty(image.ImagePath))
                        {
                            var filePath = Path.Combine(_env.WebRootPath, image.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                        }
                    }
                }

                var uploadFolder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadFolder);

                // Add new images
                if (ImageFiles != null && ImageFiles.Any())
                {
                    foreach (var imageFile in ImageFiles)
                    {
                        var uniqueFileName = $"product_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var imagePath = Path.Combine("images/products", uniqueFileName).Replace("\\", "/");
                        var fullFilePath = Path.Combine(_env.WebRootPath, imagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                        // Copy file to memory
                        byte[] imageData;
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            imageData = ms.ToArray();
                        }

                        // Save to disk
                        await System.IO.File.WriteAllBytesAsync(fullFilePath, imageData);

                        product.Images.Add(new ItemImage
                        {
                            ImageMimeType = imageFile.ContentType,
                            ImagePath = imagePath,
                            ImageData = imageData
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = product.Id });
            }

            model.ExistingImages = product.Images.ToList();
            model.Categories = _context.Categories.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("AccessDenied", "Admin");

            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Tags = product.Tags,
                ExistingImages = product.Images.ToList(),
                Categories = _context.Categories.ToList()
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // Delete image files on disk
                foreach (var image in product.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImagePath))
                    {
                        var filePath = Path.Combine(_env.WebRootPath, image.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult GetImage(int id)
        {
            var image = _context.ItemImages.FirstOrDefault(img => img.Id == id);

            if (image == null)
                return NotFound();

            byte[]? imageBytes = image.ImageData;

            if (imageBytes == null && !string.IsNullOrEmpty(image.ImagePath))
            {
                var filePath = Path.Combine(_env.WebRootPath, image.ImagePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(filePath))
                {
                    imageBytes = System.IO.File.ReadAllBytes(filePath);
                }
                else
                {
                    Console.WriteLine($"Image path not found: {filePath}");
                    return NotFound();
                }
            }

            if (imageBytes == null)
            {
                Console.WriteLine($"Image data is completely null for image ID {id}");
                return NotFound();  
            }

            var mimeType = image.ImageMimeType ?? "image/jpeg";
            return File(imageBytes, mimeType);
        }
    }
}
