using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductManager.Infrastructure.Persistence.Seeders;
using ProductManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Veritaban� migration ve seed i�lemlerini arka planda ger�ekle�tiren i�.
    /// IHostedService aray�z� ile uygulama ba�lad���nda veya durdu�unda i�lem yapar.
    /// </summary>
    public sealed class SeedJob(IServiceProvider serviceProvider, ILogger<SeedJob> logger) : IHostedService
    {
        /// <summary>
        /// Uygulama ba�lad���nda veritaban� migration ve seed i�lemlerini ba�lat�r.
        /// </summary>
        /// <param name="cancellationToken">Asenkron i�lemlerin iptali i�in kullan�lan token.</param>
        /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Seed job started."); // Ba�lang�� log'u

            // Dependency Injection (DI) ile scope olu�turulur
            using (var scope = serviceProvider.CreateScope())
            {
                // ApplicationDbContext ve Seeder servisleri al�n�r
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // Veritaban� migration i�lemi ger�ekle�tirilir
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migration completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Migration error");
                    throw; // Migration hatas� f�rlat�l�r
                }

                try
                {
                    // Seeder i�lemleri yap�l�r
                    var userSeeder = scope.ServiceProvider.GetRequiredService<UserSeeder>();
                    var productSeeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();

                    // Kullan�c� ve �r�n verileri seed edilir
                    await userSeeder.SeedAsync();
                    await productSeeder.SeedAsync();
                    logger.LogInformation("Seeding completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Seeding error");
                    throw; // Seed hatas� f�rlat�l�r
                }
            }

            logger.LogInformation("Seed job completed."); // ��lem tamamlanma log'u
        }

        /// <summary>
        /// Uygulama durdu�unda �a�r�lan metot.
        /// </summary>
        /// <param name="cancellationToken">�ptal tokeni.</param>
        /// <returns>Task.CompletedTask d�nd�r�r.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Seed job stopped."); // Durdurma log'u
            return Task.CompletedTask;
        }
    }
}
