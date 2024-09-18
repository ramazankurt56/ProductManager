using Lunavex.Result;
using MediatR;
using ProductManager.Application.Models;
using System;

namespace ProductManager.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Bir �r�n� benzersiz kimli�ine g�re almak i�in kullan�lan sorgu.
/// </summary>
public sealed class GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    /// <summary>
    /// �r�n�n benzersiz kimli�i.
    /// </summary>
    public Guid Id { get; set; }
}
