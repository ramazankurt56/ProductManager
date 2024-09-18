using FluentValidation;

namespace ProductManager.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// <see cref="CreateProductCommand"/> için doðrulayýcý sýnýf.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>
    /// <see cref="CreateProductCommandValidator"/>  classýn yeni bir instancesýný oluþturur.
    /// </summary>
    public CreateProductCommandValidator()
    {
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
