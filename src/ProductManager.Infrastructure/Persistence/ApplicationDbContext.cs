using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence;

/// <summary>
/// Uygulaman�n ana veritaban� ba�lam� (DbContext).
/// IdentityDbContext ile birlikte kimlik do�rulama i�lemlerini i�erir.
/// IUnitOfWork aray�z� ile i�lemleri (transaction) y�netir.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private readonly ILogger<ApplicationDbContext> _logger;

    /// <summary>
    /// ApplicationDbContext'in yap�c� metodu.
    /// DbContextOptions ve logger ba��ml�l�klar�n� al�r.
    /// </summary>
    /// <param name="options">Veritaban� se�enekleri.</param>
    /// <param name="logger">Logger servisi.</param>
    public ApplicationDbContext(DbContextOptions options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    /// <summary>
    /// �r�nler i�in DbSet tan�m�.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Yeni bir transaction ba�lat�r. E�er zaten bir transaction varsa, i�lem yap�lmaz.
    /// </summary>
    /// <param name="cancellationToken">�ptal tokeni.</param>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            return;
        }

        _logger.LogInformation("Starting a new transaction.");
        _transaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Ba�lat�lan transaction'� commit eder. ��lem s�ras�nda hata olursa rollback yap�l�r.
    /// </summary>
    /// <param name="cancellationToken">�ptal tokeni.</param>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
            {
                return;
            }

            _logger.LogInformation("Committing the transaction.");
            await _transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while committing the transaction. Rolling back...");
            await RollbackAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Transaction'� geri al�r (rollback).
    /// </summary>
    /// <param name="cancellationToken">�ptal tokeni.</param>
    /// <returns>Asenkron g�rev tamamland���nda Task d�ner.</returns>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction is null)
            {
                return;
            }

            _logger.LogInformation("Rolling back the transaction.");
            await _transaction.RollbackAsync();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Model olu�turma i�lemleri s�ras�nda entity yap�land�rmalar�n� uygular.
    /// </summary>
    /// <param name="builder">ModelBuilder nesnesi.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
