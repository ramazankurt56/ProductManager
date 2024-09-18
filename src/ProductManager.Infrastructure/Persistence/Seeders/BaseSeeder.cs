
namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// Tüm seeder sýnýflarý için temel soyut sýnýf.
/// Seeder iþlemlerinin nasýl yapýlacaðýný belirlemek için kullanýlýr.
/// </summary>
public abstract class BaseSeeder
{
    /// <summary>
    /// Seed iþlemini gerçekleþtirecek olan soyut metot.
    /// Her türetilen sýnýf bu metodu implemente etmelidir.
    /// </summary>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
    public abstract Task SeedAsync();
}
