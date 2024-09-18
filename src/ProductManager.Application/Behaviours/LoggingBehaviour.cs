using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ProductManager.Application.Behaviours;

/// <summary>
/// MediatR istekleri i�in loglama davran���n� tan�mlayan s�n�f.
/// </summary>
/// <typeparam name="TRequest">�stek t�r�.</typeparam>
/// <typeparam name="TResponse">Yan�t t�r�.</typeparam>
internal class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<TRequest> _logger;

    /// <summary>
    /// Yeni bir �rnek olu�turur.
    /// </summary>
    /// <param name="logger">Loglama i�in kullan�lacak ILogger �rne�i.</param>
    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// �ste�i i�ler ve loglama yapar.
    /// </summary>
    /// <param name="request">��lenecek istek.</param>
    /// <param name="next">Sonraki istek i�leyicisi.</param>
    /// <param name="cancellationToken">�ptal token�.</param>
    /// <returns>��lenmi� yan�t� d�nd�r�r.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // T�m hassas verilerin maskelemeye tabi tutulmas�
        string requestData = MaskSensitiveData(request);

        // ��lem ba�lamadan �nce loglama yap
        _logger.LogInformation($"Handling {typeof(TRequest).Name} with data: {requestData}");

        var response = await next();

        // ��lem tamamland�ktan sonra loglama yap
        _logger.LogInformation($"Handled {typeof(TRequest).Name} successfully with response: {@response}");

        return response;
    }

    /// <summary>
    /// �stek nesnesindeki hassas verileri maskeler.
    /// </summary>
    /// <param name="request">Maskelenecek istek nesnesi.</param>
    /// <returns>Maskelenmi� istek verisini d�nd�r�r.</returns>
    private string MaskSensitiveData(TRequest request)
    {
        // Varsay�lan olarak "Password", "Token" gibi hassas alanlar� maskelemek i�in liste olu�tur
        var sensitiveKeywords = new[] { "Password", "Token", "Secret", "Key" };

        var properties = request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyValues = new Dictionary<string, string>();

        foreach (var property in properties)
        {
            var value = property.GetValue(request)?.ToString() ?? "null";
            // E�er property ad� "Password", "Token" vb. ise maskelenir
            if (sensitiveKeywords.Any(keyword => property.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                propertyValues[property.Name] = "***MASKED***";
            }
            else
            {
                propertyValues[property.Name] = value;
            }
        }

        // �zellikleri isim=de�er �iftleri olarak birle�tir
        var maskedData = string.Join(", ", propertyValues.Select(kv => $"{kv.Key} = {kv.Value}"));

        return $"{typeof(TRequest).Name} {{ {maskedData} }}";
    }
}
