using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Veritabanına ürün verilerini seed eden sınıf.
/// Veritabanında ürün verileri yoksa örnek ürünler ekler.
/// </summary>
public sealed class ProductSeeder(ApplicationDbContext dbContext, ILogger<ProductSeeder> logger) : BaseSeeder
{
    /// <summary>
    /// Veritabanına ürün verilerini seed eder.
    /// Eğer ürün verileri zaten varsa, ekleme yapılmaz.
    /// </summary>
    public async override Task SeedAsync()
    {
        // Loglama: Seeding işlemi başlatıldı
        logger.LogInformation("Starting product seeding...");

        // Eğer veritabanında ürün yoksa, örnek ürünler eklenir
        if (!dbContext.Products.Any())
        {
            dbContext.Products.AddRange(
                new Product { Name = "Bilgisayar", Price = 10000, Description = "8Gb RAM ve i5 işlemci" },
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
