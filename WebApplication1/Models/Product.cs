namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public Category? Category { get; set; } = null!;

        public int StockQuantity { get; set; }
        public string? Tags { get; set; } 
        public virtual ICollection<ItemImage> Images { get; set; } = new List<ItemImage>();
    }
}
