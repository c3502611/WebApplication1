using System.Collections.Generic;

namespace WebApplication1.Models
{
    public static class MockData
    {
        public static List<Product> GetProducts() => new()
        {
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 19.99m,
                Description = "Cotton Tee",
                Category = "Shirts",
                ImageUrl = "https://via.placeholder.com/300",
                StockQuantity = 10
            },
            new Product
            {
                Id = 2,
                Name = "Hoodie",
                Price = 39.99m,
                Description = "Comfy Hoodie",
                Category = "Outerwear",
                ImageUrl = "https://via.placeholder.com/300",
                StockQuantity = 5
            }
        };

        public static List<Product> GetCartItems() => new()
        {
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 19.99m,
                Description = "Cotton Tee",
                Category = "Shirts",
                ImageUrl = "https://via.placeholder.com/300",
                StockQuantity = 1
            }
        };

        public static List<Order> GetOrders() => new()
        {
            new Order
            {
                OrderId = 1001,
                CustomerName = "Alex Tan",
                TotalAmount = 59.98m
            },
            new Order
            {
                OrderId = 1002,
                CustomerName = "Jamie Lee",
                TotalAmount = 19.99m
            }
        };
    }
}
