using Microsoft.AspNetCore.Identity;
using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Veritabanýna kullanýcý ve rol verilerini seed eden sýnýf.
/// Admin rolü ve admin kullanýcý eklenir, eðer yoksa.
/// </summary>
public sealed class UserSeeder(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<UserSeeder> logger) : BaseSeeder
{
    /// <summary>
    /// Veritabanýna kullanýcý ve rol verilerini seed eder.
    /// Admin rolü ve admin kullanýcý yoksa oluþturur, varsa hiçbir iþlem yapmaz.
    /// </summary>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
    public async override Task SeedAsync()
    {
        // Loglama: Seeding iþlemi baþlatýldý
        logger.LogInformation("Starting user seeding...");

        // Admin rolünü kontrol et, yoksa oluþtur
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role { Name = "Admin" });
            logger.LogInformation("Admin role created.");
        }
        else
        {
            logger.LogInformation("Admin role already exists.");
        }

        // Admin kullanýcýsýný kontrol et, yoksa oluþtur
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            var user = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Password123*");
            if (result.Succeeded)
            {
                // Admin rolünü kullanýcýya atama
                await userManager.AddToRoleAsync(user, "Admin");
                logger.LogInformation("Admin user created and assigned to Admin role.");
            }
            else
            {
                // Loglama: Kullanýcý oluþturulurken hata oluþtu
                logger.LogError("An error occurred while creating admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
        }
    }
}
