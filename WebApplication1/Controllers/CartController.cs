using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Helpers;
using System.Collections.Generic;
using System.Linq;

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        var product = _context.Products
            .Include(p => p.Images)
            .FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return Json(new { success = false, message = "Product not found." });
        }

        var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            string base64Image = null;
            var firstImage = product.Images.FirstOrDefault();
            if (firstImage != null && firstImage.ImageData != null && !string.IsNullOrEmpty(firstImage.ImageMimeType))
            {
                base64Image = $"data:{firstImage.ImageMimeType};base64,{Convert.ToBase64String(firstImage.ImageData)}";
            }

            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageBase64 = base64Image
            });
        }

        HttpContext.Session.SetObject("Cart", cart);

        int cartCount = cart.Sum(c => c.Quantity);
        TempData["CartCount"] = cartCount;

        TempData.Keep("User");
        TempData.Keep("Role");

        return Json(new { success = true, cartCount });
    }

    public IActionResult Index()
    {
        TempData.Keep("User");
        TempData.Keep("Role");

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        int cartCount = cart.Sum(c => c.Quantity);
        TempData["CartCount"] = cartCount;

        return View(cart);
    }

    public IActionResult Remove(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObject("Cart", cart);
            TempData["CartCount"] = cart.Sum(c => c.Quantity);
        }

        TempData.Keep("User");
        TempData.Keep("Role");

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        if (!cart.Any())
        {
            TempData["Message"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        return View(new CheckoutInfo());
    }

    [HttpPost]
    public IActionResult Checkout(CheckoutInfo model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill out all required fields.";
            return View(model);
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Cart is empty.";
            return RedirectToAction("Index");
        }

        foreach (var item in cart)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null)
            {
                TempData["Error"] = $"Product ID {item.ProductId} no longer exists.";
                return RedirectToAction("Index");
            }
            if (product.StockQuantity < item.Quantity)
            {
                TempData["Error"] = $"Not enough stock for {product.Name}. Available: {product.StockQuantity}";
                return RedirectToAction("Index");
            }

            product.StockQuantity -= item.Quantity;
        }

        _context.SaveChanges();

        HttpContext.Session.Remove("Cart");
        TempData["CartCount"] = 0;

        TempData["Success"] = $"Order placed for {model.FullName}!";

        return RedirectToAction("Confirm");
    }

    public IActionResult Confirm()
    {
        TempData.Keep("User");
        TempData.Keep("Role");

        ViewBag.Message = "Thank you! Your order has been placed.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity, string change)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null)
        {
            if (change == "increment")
            {
                item.Quantity++;
            }
            else if (change == "decrement" && item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                if (quantity >= 1)
                {
                    item.Quantity = quantity;
                }
            }
        }

        HttpContext.Session.SetObject("Cart", cart);
        TempData["CartCount"] = cart.Sum(c => c.Quantity);

        TempData.Keep("User");
        TempData.Keep("Role");

        return Json(new
        {
            totalPrice = item.Price * item.Quantity,
            cartTotal = cart.Sum(i => i.Price * i.Quantity),
            updatedQuantity = item.Quantity
        });
    }
}
