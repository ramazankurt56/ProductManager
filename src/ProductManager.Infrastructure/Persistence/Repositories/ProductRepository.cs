using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

namespace ProductManager.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    /// <summary>
    /// Veritaban�na yeni bir �r�n ekler.
    /// </summary>
    /// <param name="product">Eklenecek �r�n nesnesi.</param>
    /// <returns>Asenkron i�lem tamamland���nda Task d�ner.</returns>
    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip �r�n� veritaban�ndan siler.
    /// </summary>
    /// <param name="id">Silinecek �r�n�n ID'si.</param>
    /// <returns>Asenkron i�lem tamamland���nda Task d�ner.</returns>
    public async Task DeleteAsync(Guid id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
        }
    }

    /// <summary>
    /// Veritaban�ndaki t�m �r�nleri getirir.
    /// </summary>
    /// <returns>T�m �r�nlerin listesini d�ner.</returns>
    public async Task<List<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    /// <summary>
    /// Belirtilen ID'ye sahip �r�n� getirir.
    /// </summary>
    /// <param name="id">Aranan �r�n�n ID'si.</param>
    /// <returns>�r�n bulunduysa o �r�n, bulunmad�ysa null d�ner.</returns>
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// �r�n ad�yla arama yaparak, belirtilen ada sahip �r�n� getirir.
    /// </summary>
    /// <param name="name">Aranan �r�n�n ad�.</param>
    /// <returns>�r�n bulunduysa o �r�n, bulunmad�ysa null d�ner.</returns>
    public async Task<Product?> GetByNameAsync(string name)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Name == name);
    }

    /// <summary>
    /// Asenkron olarak var olan bir �r�n� g�nceller.
    /// </summary>
    /// <param name="product">G�ncellenecek �r�n nesnesi.</param>
    /// <returns>Task d�ner. Bu y�ntem bir de�er d�nd�rmez.</returns>
    public Task UpdateAsync(Product product)
    {
        context.Products.Update(product);

        return Task.CompletedTask;
    }
}
