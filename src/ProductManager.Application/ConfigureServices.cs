
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Behaviours;
using ProductManager.Application.Profiles;
using System.Reflection;

namespace ProductManager.Application;

/// <summary>
/// Uygulama katman�n�n servislerini yap�land�ran statik s�n�f.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Uygulama servislerini DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <returns>G�ncellenmi� servis koleksiyonu.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Assembly'deki t�m do�rulay�c�lar� ekle
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR servislerini ekle ve �zel davran��lar� yap�land�r
        services.AddMediatR(config =>
        {
            // Mevcut assembly'deki servisleri kaydet
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Do�rulama ve loglama davran��lar�n� ekle
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });

        // AutoMapper profillerini ekle
        services.AddAutoMapper(typeof(ProductProfile));

        // Servis koleksiyonunu d�nd�r
        return services;
    }
}