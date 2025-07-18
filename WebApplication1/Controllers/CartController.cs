using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data; // for AppDbContext
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
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
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

    public IActionResult Checkout()
    {
        TempData.Keep("User");
        TempData.Keep("Role");

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        if (!cart.Any())
        {
            TempData["Message"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        return View("Checkout", cart);
    }

    public IActionResult Confirm()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        if (!cart.Any())
        {
            TempData["Message"] = "Your cart is empty.";
            return RedirectToAction("Index");
        }

        HttpContext.Session.Remove("Cart");

        TempData.Keep("User");
        TempData.Keep("Role");
        ViewBag.Message = "Thank you! Your order has been placed.";

        return View(cart);
    }
}
