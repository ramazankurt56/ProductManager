using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ProductManager.Application.Behaviours;

/// <summary>
/// MediatR istekleri için loglama davranýþýný tanýmlayan sýnýf.
/// </summary>
/// <typeparam name="TRequest">Ýstek türü.</typeparam>
/// <typeparam name="TResponse">Yanýt türü.</typeparam>
internal class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<TRequest> _logger;

    /// <summary>
    /// Yeni bir örnek oluþturur.
    /// </summary>
    /// <param name="logger">Loglama için kullanýlacak ILogger örneði.</param>
    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ýsteði iþler ve loglama yapar.
    /// </summary>
    /// <param name="request">Ýþlenecek istek.</param>
    /// <param name="next">Sonraki istek iþleyicisi.</param>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    /// <returns>Ýþlenmiþ yanýtý döndürür.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Tüm hassas verilerin maskelemeye tabi tutulmasý
        string requestData = MaskSensitiveData(request);

        // Ýþlem baþlamadan önce loglama yap
        _logger.LogInformation($"Handling {typeof(TRequest).Name} with data: {requestData}");

        var response = await next();

        // Ýþlem tamamlandýktan sonra loglama yap
        _logger.LogInformation($"Handled {typeof(TRequest).Name} successfully with response: {@response}");

        return response;
    }

    /// <summary>
    /// Ýstek nesnesindeki hassas verileri maskeler.
    /// </summary>
    /// <param name="request">Maskelenecek istek nesnesi.</param>
    /// <returns>Maskelenmiþ istek verisini döndürür.</returns>
    private string MaskSensitiveData(TRequest request)
    {
        // Varsayýlan olarak "Password", "Token" gibi hassas alanlarý maskelemek için liste oluþtur
        var sensitiveKeywords = new[] { "Password", "Token", "Secret", "Key" };

        var properties = request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyValues = new Dictionary<string, string>();

        foreach (var property in properties)
        {
            var value = property.GetValue(request)?.ToString() ?? "null";
            // Eðer property adý "Password", "Token" vb. ise maskelenir
            if (sensitiveKeywords.Any(keyword => property.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                propertyValues[property.Name] = "***MASKED***";
            }
            else
            {
                propertyValues[property.Name] = value;
            }
        }

        // Özellikleri isim=deðer çiftleri olarak birleþtir
        var maskedData = string.Join(", ", propertyValues.Select(kv => $"{kv.Key} = {kv.Value}"));

        return $"{typeof(TRequest).Name} {{ {maskedData} }}";
    }
}
