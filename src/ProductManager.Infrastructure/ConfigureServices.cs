using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using ProductManager.Infrastructure.BackgroundJobs;
using ProductManager.Infrastructure.Options;
using ProductManager.Infrastructure.Persistence;
using ProductManager.Infrastructure.Persistence.Repositories;
using ProductManager.Infrastructure.Persistence.Seeders;
using ProductManager.Infrastructure.Services;

namespace ProductManager.Infrastructure;

/// <summary>
/// Uygulaman�n altyap� servislerini yap�land�ran statik s�n�f.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Altyap� servislerini DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Yap�land�rma ayarlar�.</param>
    /// <returns>G�ncellenmi� servis koleksiyonu.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Uygulama veritaban� ba�lam�n� ekle ve SQL Server kullan.
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Birim i� (UnitOfWork) desenini uygulamak i�in ApplicationDbContext'i IUnitOfWork olarak kaydet.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // �r�n depolama s�n�f�n� ekle.
        services.AddScoped<IProductRepository, ProductRepository>();

        // JWT sa�lay�c�s�n� ekle.
        services.AddScoped<IJwtProvider, JwtProvider>();

        // JWT se�eneklerini yap�land�r.
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtTokenOptionsSetup>();

        // Identity servislerini ve varsay�lan token sa�lay�c�lar�n� ekle.
        services.AddIdentity<User, Role>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

        // Kimlik do�rulama ve yetkilendirme servislerini ekle.
        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorizationBuilder();

        // Servis koleksiyonunu d�nd�r.
        return services;
    }

    /// <summary>
    /// Veri tohumlay�c�lar�n� (seeders) DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <returns>G�ncellenmi� servis koleksiyonu.</returns>
    public static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        // arka plan hizmetini ekle.
        services.AddHostedService<SeedJob>();

        // Kullan�c� seeder�n� ekle.
        services.AddScoped<UserSeeder>();

        // �r�n seeder�n� ekle.
        services.AddScoped<ProductSeeder>();

        // Servis koleksiyonunu d�nd�r.
        return services;
    }
}
