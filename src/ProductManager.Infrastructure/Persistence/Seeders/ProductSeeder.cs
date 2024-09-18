using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Veritabanýna ürün verilerini seed eden sýnýf.
/// Veritabanýnda ürün verileri yoksa örnek ürünler ekler.
/// </summary>
public sealed class ProductSeeder(ApplicationDbContext dbContext, ILogger<ProductSeeder> logger) : BaseSeeder
{
    /// <summary>
    /// Veritabanýna ürün verilerini seed eder.
    /// Eðer ürün verileri zaten varsa, ekleme yapýlmaz.
    /// </summary>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
    public async override Task SeedAsync()
    {
        // Loglama: Seeding iþlemi baþlatýldý
        logger.LogInformation("Starting product seeding...");

        // Eðer veritabanýnda ürün yoksa, örnek ürünler eklenir
        if (!dbContext.Products.Any())
        {
            dbContext.Products.AddRange(
                new Product { Name = "Bilgisayar", Price = 10000, Description = "8Gb RAM ve i5 iþlemci" },
                new Product { Name = "Monitör", Price = 2000, Description = "Asus ekran" },
                new Product { Name = "Klavye", Price = 300, Description = "Q Klavye" }
            );

            await dbContext.SaveChangesAsync();
            logger.LogInformation("Products successfully seeded.");
        }
        else
        {
            logger.LogInformation("Products already exist, no seeding necessary.");
        }
    }
}
