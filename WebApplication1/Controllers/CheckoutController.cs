using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

public class CheckoutController : Controller
{
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        if (!cart.Any())
        {
            TempData["Message"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        TempData.Keep("User");
        TempData.Keep("Role");

        return View(cart);
    }
}
