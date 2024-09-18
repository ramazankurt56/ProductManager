using Lunavex.Result;
using MediatR;
using ProductManager.Application.Models;
using System;

namespace ProductManager.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Bir ürünü benzersiz kimliðine göre almak için kullanýlan sorgu.
/// </summary>
public sealed class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    /// <summary>
    /// Ürünün benzersiz kimliði.
    /// </summary>
    public Guid Id { get; set; }
}
