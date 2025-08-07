using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class ImageMigrationService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ImageMigrationService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task MigrateImagesToFilesAsync()
    {
      string imageFolder = Path.Combine(_env.WebRootPath, "images", "products");
       
        if (!Directory.Exists(imageFolder))
            Directory.CreateDirectory(imageFolder);

        var imagesToMigrate = _context.ItemImages
            .Where(img => img.ImageData != null && string.IsNullOrEmpty(img.ImagePath))
            .ToList();

        foreach (var image in imagesToMigrate)
        {
            string fileName = $"product_{image.Id}_{Guid.NewGuid()}.png";
            string fullPath = Path.Combine(imageFolder, fileName);

            await File.WriteAllBytesAsync(fullPath, image.ImageData);

            image.ImagePath = Path.Combine("images/products", fileName).Replace("\\", "/");
            image.ImageData = null; // Optional: remove image data from DB
        }

        await _context.SaveChangesAsync();
      
    }
}
