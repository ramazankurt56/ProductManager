using FluentValidation;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// <see cref="UpdateProductCommand"/> için doðrulayýcý sýnýf.
/// </summary>
public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>
    /// <see cref="UpdateProductCommandValidator"/> classýn yeni bir instancesi oluþturur
    /// </summary>
    public UpdateProductCommandValidator()
    {
        // Ürün ID'si için doðrulama kuralý
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        // Ürün adý için doðrulama kuralý
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        // Ürün açýklamasý için doðrulama kuralý
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        // Ürün fiyatý için doðrulama kuralý
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .PrecisionScale(18, 2, true).WithMessage("Price must have up to 18 digits in total, with 2 decimal places.");
    }
}
