using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();

        [Display(Name = "Tags (comma-separated)")]
        public string? Tags { get; set; }


        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        [Display(Name = "Upload Images")]
        public List<IFormFile> ImageFiles { get; set; } = new();

        public List<ItemImage> ExistingImages { get; set; } = new();

        public List<int> ImagesToRemove { get; set; } = new();
    }
}
