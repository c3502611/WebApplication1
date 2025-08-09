namespace WebApplication1.Models
{
    public class ItemImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public byte[]? ImageData { get; set; } 
        public string? ImageMimeType { get; set; }  
        public string? ImagePath { get; set; } 
    }
}
