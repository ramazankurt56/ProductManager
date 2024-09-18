using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductManager.Infrastructure.Persistence.Seeders;
using ProductManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Veritabaný migration ve seed iþlemlerini arka planda gerçekleþtiren iþ.
    /// IHostedService arayüzü ile uygulama baþladýðýnda veya durduðunda iþlem yapar.
    /// </summary>
    public sealed class SeedJob(IServiceProvider serviceProvider, ILogger<SeedJob> logger) : IHostedService
    {
        /// <summary>
        /// Uygulama baþladýðýnda veritabaný migration ve seed iþlemlerini baþlatýr.
        /// </summary>
        /// <param name="cancellationToken">Asenkron iþlemlerin iptali için kullanýlan token.</param>
        /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Seed job started."); // Baþlangýç log'u

            // Dependency Injection (DI) ile scope oluþturulur
            using (var scope = serviceProvider.CreateScope())
            {
                // ApplicationDbContext ve Seeder servisleri alýnýr
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // Veritabaný migration iþlemi gerçekleþtirilir
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migration completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Migration error");
                    throw; // Migration hatasý fýrlatýlýr
                }

                try
                {
                    // Seeder iþlemleri yapýlýr
                    var userSeeder = scope.ServiceProvider.GetRequiredService<UserSeeder>();
                    var productSeeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();

                    // Kullanýcý ve ürün verileri seed edilir
                    await userSeeder.SeedAsync();
                    await productSeeder.SeedAsync();
                    logger.LogInformation("Seeding completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Seeding error");
                    throw; // Seed hatasý fýrlatýlýr
                }
            }

            logger.LogInformation("Seed job completed."); // Ýþlem tamamlanma log'u
        }

        /// <summary>
        /// Uygulama durduðunda çaðrýlan metot.
        /// </summary>
        /// <param name="cancellationToken">Ýptal tokeni.</param>
        /// <returns>Task.CompletedTask döndürür.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Seed job stopped."); // Durdurma log'u
            return Task.CompletedTask;
        }
    }
}
