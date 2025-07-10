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
                ImageUrl = "https://assets.manufactum.de/p/207/207945/207945_01.jpg/mens-t-shirt-cotton.jpg?profile=pdsmain_1500",
                StockQuantity = 10
            },
            new Product
            {
                Id = 2,
                Name = "Hoodie",
                Price = 39.99m,
                Description = "Comfy Hoodie",
                Category = "Outerwear",
                ImageUrl = "https://www.davidgandywellwear.com/cdn/shop/products/Ultimate-Loopback-Hoodie-Black.jpg?v=1677258480&width=1946",
                StockQuantity = 5
            },
            new Product
            {                Id = 3,
                Name = "Sneakers",
                Price = 59.99m,
                Description = "Running Shoes",
                Category = "Footwear",
                ImageUrl = "http://assets.hermes.com/is/image/hermesproduct/quicker-sneaker--102190ZH09-worn-1-0-0-800-800_g.jpg",
                StockQuantity = 8
            },
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
