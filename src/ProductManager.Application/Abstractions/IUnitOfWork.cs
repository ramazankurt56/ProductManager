namespace ProductManager.Application.Abstractions;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Uygulama i�in birim i� (Unit of Work) desenini tan�mlayan interface.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// De�i�iklikleri asenkron olarak veritaban�na kaydeder.
    /// </summary>
    /// <param name="cancellationToken">�ptal token�.</param>
    /// <returns>Kaydedilen de�i�ikliklerin say�s�n� d�nd�r�r.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak yeni bir veritaban� i�lemi ba�lat�r.
    /// </summary>
    /// <param name="cancellationToken">�ptal token�.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak veritaban� i�lemini geri al�r.
    /// </summary>
    /// <param name="cancellationToken">�ptal token�.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asenkron olarak veritaban� i�lemini onaylar.
    /// </summary>
    /// <param name="cancellationToken">�ptal token�.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
