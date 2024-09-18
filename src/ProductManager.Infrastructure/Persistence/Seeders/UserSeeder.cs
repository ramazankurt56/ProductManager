using Microsoft.AspNetCore.Identity;
using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Veritaban�na kullan�c� ve rol verilerini seed eden s�n�f.
/// Admin rol� ve admin kullan�c� eklenir, e�er yoksa.
/// </summary>
public sealed class UserSeeder(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<UserSeeder> logger) : BaseSeeder
{
    /// <summary>
    /// Veritaban�na kullan�c� ve rol verilerini seed eder.
    /// Admin rol� ve admin kullan�c� yoksa olu�turur, varsa hi�bir i�lem yapmaz.
    /// </summary>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public async override Task SeedAsync()
    {
        // Loglama: Seeding i�lemi ba�lat�ld�
        logger.LogInformation("Starting user seeding...");

        // Admin rol�n� kontrol et, yoksa olu�tur
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new Role { Name = "Admin" });
            logger.LogInformation("Admin role created.");
        }
        else
        {
            logger.LogInformation("Admin role already exists.");
        }

        // Admin kullan�c�s�n� kontrol et, yoksa olu�tur
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
                // Admin rol�n� kullan�c�ya atama
                await userManager.AddToRoleAsync(user, "Admin");
                logger.LogInformation("Admin user created and assigned to Admin role.");
            }
            else
            {
                // Loglama: Kullan�c� olu�turulurken hata olu�tu
                logger.LogError("An error occurred while creating admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
        }
    }
}
