using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for your entities
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category seeding
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Shirts" },
                new Category { Id = 2, Name = "Outerwear" },
                new Category { Id = 3, Name = "Pants" },
                new Category { Id = 4, Name = "Jeans" },
                new Category { Id = 5, Name = "Dress" },
                new Category { Id = 6, Name = "Skirt" },
                new Category { Id = 7, Name = "Socks" }
            );

            // One-to-many: Product has many ItemImages
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: Category has many Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull); 

            // Optional: Make Tags column optional and max length
            modelBuilder.Entity<Product>()
                .Property(p => p.Tags)
                .HasMaxLength(500)
                .IsRequired(false);

            modelBuilder.Entity<ItemImage>()
        .HasOne(i => i.Product)
        .WithMany(p => p.Images)
        .HasForeignKey(i => i.ProductId)
        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
