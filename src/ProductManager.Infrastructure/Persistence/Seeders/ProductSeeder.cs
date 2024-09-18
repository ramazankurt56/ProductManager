using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Veritaban�na �r�n verilerini seed eden s�n�f.
/// Veritaban�nda �r�n verileri yoksa �rnek �r�nler ekler.
/// </summary>
public sealed class ProductSeeder(ApplicationDbContext dbContext, ILogger<ProductSeeder> logger) : BaseSeeder
{
    /// <summary>
    /// Veritaban�na �r�n verilerini seed eder.
    /// E�er �r�n verileri zaten varsa, ekleme yap�lmaz.
    /// </summary>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public async override Task SeedAsync()
    {
        // Loglama: Seeding i�lemi ba�lat�ld�
        logger.LogInformation("Starting product seeding...");

        // E�er veritaban�nda �r�n yoksa, �rnek �r�nler eklenir
        if (!dbContext.Products.Any())
        {
            dbContext.Products.AddRange(
                new Product { Name = "Bilgisayar", Price = 10000, Description = "8Gb RAM ve i5 i�lemci" },
                new Product { Name = "Monit�r", Price = 2000, Description = "Asus ekran" },
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
