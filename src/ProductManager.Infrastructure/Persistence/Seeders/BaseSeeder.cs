
namespace ProductManager.Infrastructure.Persistence.Seeders;

/// <summary>
/// T�m seeder s�n�flar� i�in temel soyut s�n�f.
/// Seeder i�lemlerinin nas�l yap�laca��n� belirlemek i�in kullan�l�r.
/// </summary>
public abstract class BaseSeeder
{
    /// <summary>
    /// Seed i�lemini ger�ekle�tirecek olan soyut metot.
    /// Her t�retilen s�n�f bu metodu implemente etmelidir.
    /// </summary>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public abstract Task SeedAsync();
}
