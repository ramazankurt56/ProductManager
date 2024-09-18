using FluentValidation;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// <see cref="DeleteProductCommand"/> i�in do�rulay�c� s�n�f.
/// </summary>
public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    /// <summary>
    /// <see cref="DeleteProductCommandValidator"/> class�n yeni bir instances�n� olu�turur.
    /// </summary>
    public DeleteProductCommandValidator()
    {
        // �r�n ID'si i�in do�rulama kural�
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}
