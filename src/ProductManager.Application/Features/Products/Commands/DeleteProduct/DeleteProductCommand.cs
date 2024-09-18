using Lunavex.Result;
using MediatR;
using System;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Mevcut bir ürünü silmek için kullanýlan komut.
/// </summary>
public sealed class DeleteProductCommand : IRequest<Result<string>>
{
    /// <summary>
    /// Silinecek ürünün benzersiz kimliði.
    /// </summary>
    public Guid Id { get; set; }
}
