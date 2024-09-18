using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

namespace ProductManager.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    /// <summary>
    /// Veritabanýna yeni bir ürün ekler.
    /// </summary>
    /// <param name="product">Eklenecek ürün nesnesi.</param>
    /// <returns>Asenkron iþlem tamamlandýðýnda Task döner.</returns>
    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip ürünü veritabanýndan siler.
    /// </summary>
    /// <param name="id">Silinecek ürünün ID'si.</param>
    /// <returns>Asenkron iþlem tamamlandýðýnda Task döner.</returns>
    public async Task DeleteAsync(Guid id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
        }
    }

    /// <summary>
    /// Veritabanýndaki tüm ürünleri getirir.
    /// </summary>
    /// <returns>Tüm ürünlerin listesini döner.</returns>
    public async Task<List<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    /// <summary>
    /// Belirtilen ID'ye sahip ürünü getirir.
    /// </summary>
    /// <param name="id">Aranan ürünün ID'si.</param>
    /// <returns>Ürün bulunduysa o ürün, bulunmadýysa null döner.</returns>
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Ürün adýyla arama yaparak, belirtilen ada sahip ürünü getirir.
    /// </summary>
    /// <param name="name">Aranan ürünün adý.</param>
    /// <returns>Ürün bulunduysa o ürün, bulunmadýysa null döner.</returns>
    public async Task<Product?> GetByNameAsync(string name)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Name == name);
    }

    /// <summary>
    /// Asenkron olarak var olan bir ürünü günceller.
    /// </summary>
    /// <param name="product">Güncellenecek ürün nesnesi.</param>
    /// <returns>Task döner. Bu yöntem bir deðer döndürmez.</returns>
    public Task UpdateAsync(Product product)
    {
        context.Products.Update(product);

        return Task.CompletedTask;
    }
}
