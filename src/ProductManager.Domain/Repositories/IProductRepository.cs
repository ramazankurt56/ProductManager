using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Repositories;

/// <summary>
/// Ürünlerle ilgili veri erişim işlemlerini tanımlayan arayüz.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Tüm ürünleri asenkron olarak getirir.
    /// </summary>
    /// <returns>Ürünlerin listesini döndürür.</returns>
    Task<List<Product>> GetAllAsync();

    /// <summary>
    /// Belirtilen kimliğe sahip ürünü asenkron olarak getirir.
    /// </summary>
    /// <param name="id">Ürünün benzersiz kimliği.</param>
    /// <returns>Ürün nesnesini veya null değerini döndürür.</returns>
    Task<Product?> GetByIdAsync(Guid id);

    /// <summary>
    /// Belirtilen ada sahip ürünü asenkron olarak getirir.
    /// </summary>
    /// <param name="name">Aranan ürünün adı.</param>
    /// <returns>Ürün nesnesini veya null değerini döndürür.</returns>
    Task<Product?> GetByNameAsync(string name);

    /// <summary>
    /// Yeni bir ürün ekler.
    /// </summary>
    /// <param name="product">Eklenecek ürün nesnesi.</param>
    Task AddAsync(Product product);

    /// <summary>
    /// Mevcut bir ürünü günceller.
    /// </summary>
    /// <param name="product">Güncellenecek ürün nesnesi.</param>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Belirtilen kimliğe sahip ürünü siler.
    /// </summary>
    /// <param name="id">Silinecek ürünün benzersiz kimliği.</param>
    Task DeleteAsync(Guid id);
}
