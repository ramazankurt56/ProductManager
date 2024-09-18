using Lunavex.Result;
using MediatR;

namespace ProductManager.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Yeni bir �r�n olu�turmak i�in kullan�lan komut.
/// </summary>
/// <param name="Name">�r�n�n ad�.</param>
/// <param name="Price">�r�n�n fiyat�.</param>
/// <param name="Description">�r�n�n a��klamas�.</param>
public sealed record CreateProductCommand(string Name, decimal Price, string Description) : IRequest<Result<Guid>>;
