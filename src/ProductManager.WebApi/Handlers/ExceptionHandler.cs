
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace ProductManager.WebApi.Handlers;

// ExceptionHandler sınıfı, istisnaları özel olarak ele almak için kullanılır.
public class ExceptionHandler : IExceptionHandler
{

    private readonly ILogger<ExceptionHandler> _logger;

    // Yapıcı metod, ILogger bağımlılığını alır.
    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    // İstisnaları asenkron olarak ele alan metod.
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Eğer istisna bir ValidationException ise
        if (exception is ValidationException validationException)
        {
            // Uyarı seviyesinde log kaydı oluşturur.
            _logger.LogWarning("Validation error occurred: {Message}", validationException.Message);

            // Problem detaylarını hazırlar.
            var problemDetails = new ProblemDetails
            {
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationException.Message,
                Extensions = new Dictionary<string, object?>
                    {
                        { "errors", validationException.Errors }
                    }
            };

            // HTTP yanıt durum kodunu ayarlar.
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            // Problem detaylarını JSON olarak yanıtlar.
            await httpContext.Response.WriteAsJsonAsync(problemDetails);

            return true;
        }

        // Diğer tüm istisnalar için hata log kaydı oluşturur.
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        // 500 Internal Server Error için problem detaylarını hazırlar.


        var problemDetails500 = new ProblemDetails
        {
            Title = "Internal server error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message
        };

        // HTTP yanıt durum kodunu ayarlar.
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Problem detaylarını JSON olarak yanıtlar.
        await httpContext.Response.WriteAsJsonAsync(problemDetails500);

        return true;
    }
}
