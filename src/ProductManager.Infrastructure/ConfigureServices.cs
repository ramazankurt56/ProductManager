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
/// Uygulamanýn altyapý servislerini yapýlandýran statik sýnýf.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Altyapý servislerini DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configuration">Yapýlandýrma ayarlarý.</param>
    /// <returns>Güncellenmiþ servis koleksiyonu.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Uygulama veritabaný baðlamýný ekle ve SQL Server kullan.
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Birim iþ (UnitOfWork) desenini uygulamak için ApplicationDbContext'i IUnitOfWork olarak kaydet.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Ürün depolama sýnýfýný ekle.
        services.AddScoped<IProductRepository, ProductRepository>();

        // JWT saðlayýcýsýný ekle.
        services.AddScoped<IJwtProvider, JwtProvider>();

        // JWT seçeneklerini yapýlandýr.
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtTokenOptionsSetup>();

        // Identity servislerini ve varsayýlan token saðlayýcýlarýný ekle.
        services.AddIdentity<User, Role>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

        // Kimlik doðrulama ve yetkilendirme servislerini ekle.
        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorizationBuilder();

        // Servis koleksiyonunu döndür.
        return services;
    }

    /// <summary>
    /// Veri tohumlayýcýlarýný (seeders) DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <returns>Güncellenmiþ servis koleksiyonu.</returns>
    public static IServiceCollection AddSeeders(this IServiceCollection services)
    {
        // arka plan hizmetini ekle.
        services.AddHostedService<SeedJob>();

        // Kullanýcý seederýný ekle.
        services.AddScoped<UserSeeder>();

        // Ürün seederýný ekle.
        services.AddScoped<ProductSeeder>();

        // Servis koleksiyonunu döndür.
        return services;
    }
}
