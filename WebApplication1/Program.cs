using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<ImageMigrationService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Username = "admin",
            Email = "admin@a",
            Role = "Admin"
        };
        admin.Password = hasher.HashPassword(admin, "password");

        context.Users.Add(admin);
        context.SaveChanges();
    }

    var migrationService = scope.ServiceProvider.GetRequiredService<ImageMigrationService>();
    await migrationService.MigrateImagesToFilesAsync();
}

app.Run();
