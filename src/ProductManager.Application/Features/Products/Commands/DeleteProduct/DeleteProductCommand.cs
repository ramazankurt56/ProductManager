using Lunavex.Result;
using MediatR;
using System;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Mevcut bir �r�n� silmek i�in kullan�lan komut.
/// </summary>
public sealed class DeleteProductCommand : IRequest<Result<string>>
{
    /// <summary>
    /// Silinecek �r�n�n benzersiz kimli�i.
    /// </summary>
    public Guid Id { get; set; }
}
