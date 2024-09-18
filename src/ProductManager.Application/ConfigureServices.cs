
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Application.Behaviours;
using ProductManager.Application.Profiles;
using System.Reflection;

namespace ProductManager.Application;

/// <summary>
/// Uygulama katmanýnýn servislerini yapýlandýran statik sýnýf.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Uygulama servislerini DI konteynerine ekler.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <returns>Güncellenmiþ servis koleksiyonu.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Assembly'deki tüm doðrulayýcýlarý ekle
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR servislerini ekle ve özel davranýþlarý yapýlandýr
        services.AddMediatR(config =>
        {
            // Mevcut assembly'deki servisleri kaydet
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Doðrulama ve loglama davranýþlarýný ekle
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });

        // AutoMapper profillerini ekle
        services.AddAutoMapper(typeof(ProductProfile));

        // Servis koleksiyonunu döndür
        return services;
    }
}