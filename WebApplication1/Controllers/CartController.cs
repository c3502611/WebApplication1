using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
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

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            TempData["Message"] = "Product not found.";
            return RedirectToAction("Index", "Home");
        }

        var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            string base64Image = null;
            if (product.ImageData != null && !string.IsNullOrEmpty(product.ImageMimeType))
            {
                base64Image = $"data:{product.ImageMimeType};base64,{Convert.ToBase64String(product.ImageData)}";
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

        TempData.Keep("User");
        TempData.Keep("Role");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Index()
    {
        TempData.Keep("User");
        TempData.Keep("Role");

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
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
}
