using FluentValidation;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// <see cref="UpdateProductCommand"/> i�in do�rulay�c� s�n�f.
/// </summary>
public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>
    /// <see cref="UpdateProductCommandValidator"/> class�n yeni bir instancesi olu�turur
    /// </summary>
    public UpdateProductCommandValidator()
    {
        // �r�n ID'si i�in do�rulama kural�
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        // �r�n ad� i�in do�rulama kural�
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        // �r�n a��klamas� i�in do�rulama kural�
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        // �r�n fiyat� i�in do�rulama kural�
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .PrecisionScale(18, 2, true).WithMessage("Price must have up to 18 digits in total, with 2 decimal places.");
    }
}
