using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ProductManager.Infrastructure.Persistence;

/// <summary>
/// Uygulamanýn ana veritabaný baðlamý (DbContext).
/// IdentityDbContext ile birlikte kimlik doðrulama iþlemlerini içerir.
/// IUnitOfWork arayüzü ile iþlemleri (transaction) yönetir.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private readonly ILogger<ApplicationDbContext> _logger;

    /// <summary>
    /// ApplicationDbContext'in yapýcý metodu.
    /// DbContextOptions ve logger baðýmlýlýklarýný alýr.
    /// </summary>
    /// <param name="options">Veritabaný seçenekleri.</param>
    /// <param name="logger">Logger servisi.</param>
    public ApplicationDbContext(DbContextOptions options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ürünler için DbSet tanýmý.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Yeni bir transaction baþlatýr. Eðer zaten bir transaction varsa, iþlem yapýlmaz.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokeni.</param>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
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
    /// Baþlatýlan transaction'ý commit eder. Ýþlem sýrasýnda hata olursa rollback yapýlýr.
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokeni.</param>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
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
    /// Transaction'ý geri alýr (rollback).
    /// </summary>
    /// <param name="cancellationToken">Ýptal tokeni.</param>
    /// <returns>Asenkron görev tamamlandýðýnda Task döner.</returns>
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
    /// Model oluþturma iþlemleri sýrasýnda entity yapýlandýrmalarýný uygular.
    /// </summary>
    /// <param name="builder">ModelBuilder nesnesi.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}
