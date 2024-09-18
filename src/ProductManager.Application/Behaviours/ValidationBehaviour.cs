using FluentValidation.Results;
using FluentValidation;
using MediatR;

namespace ProductManager.Application.Behaviours;

/// <summary>
/// MediatR istekleri i�in do�rulama davran���n� tan�mlayan class.
/// </summary>
/// <typeparam name="TRequest">request t�r�.</typeparam>
/// <typeparam name="TResponse">response t�r�.</typeparam>
internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Yeni bir instance olu�turur.
    /// </summary>
    /// <param name="validators">validators listesi.</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// �ste�i i�ler ve do�rulama yapar.
    /// </summary>
    /// <param name="request">��lenecek istek.</param>
    /// <param name="next">Sonraki istek i�leyicisi.</param>
    /// <param name="cancellationToken">�ptal token�.</param>
    /// <returns>��lenmi� yan�t� d�nd�r�r.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // E�er validator yoksa, bir sonraki i�leyiciye ge�
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Validators �al��t�r ve hatalar� topla
        List<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        // E�er hata varsa, ValidationException f�rlat
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Bir sonraki i�leyiciye ge�
        return await next();
    }
}
