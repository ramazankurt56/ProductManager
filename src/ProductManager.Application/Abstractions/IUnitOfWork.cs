namespace ProductManager.Application.Abstractions;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Uygulama için birim iþ (Unit of Work) desenini tanýmlayan interface.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Deðiþiklikleri asenkron olarak veritabanýna kaydeder.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    /// <returns>Kaydedilen deðiþikliklerin sayýsýný döndürür.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak yeni bir veritabaný iþlemi baþlatýr.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak veritabaný iþlemini geri alýr.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak veritabaný iþlemini onaylar.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
