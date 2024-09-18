using FluentValidation;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// <see cref="DeleteProductCommand"/> için doðrulayýcý sýnýf.
/// </summary>
public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    /// <summary>
    /// <see cref="DeleteProductCommandValidator"/> classýn yeni bir instancesýný oluþturur.
    /// </summary>
    public DeleteProductCommandValidator()
    {
        // Ürün ID'si için doðrulama kuralý
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}
