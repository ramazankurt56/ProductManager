using FluentValidation.Results;
using FluentValidation;
using MediatR;

namespace ProductManager.Application.Behaviours;

/// <summary>
/// MediatR istekleri için doðrulama davranýþýný tanýmlayan class.
/// </summary>
/// <typeparam name="TRequest">request türü.</typeparam>
/// <typeparam name="TResponse">response türü.</typeparam>
internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Yeni bir instance oluþturur.
    /// </summary>
    /// <param name="validators">validators listesi.</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Ýsteði iþler ve doðrulama yapar.
    /// </summary>
    /// <param name="request">Ýþlenecek istek.</param>
    /// <param name="next">Sonraki istek iþleyicisi.</param>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    /// <returns>Ýþlenmiþ yanýtý döndürür.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Eðer validator yoksa, bir sonraki iþleyiciye geç
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Validators çalýþtýr ve hatalarý topla
        List<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        // Eðer hata varsa, ValidationException fýrlat
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Bir sonraki iþleyiciye geç
        return await next();
    }
}
