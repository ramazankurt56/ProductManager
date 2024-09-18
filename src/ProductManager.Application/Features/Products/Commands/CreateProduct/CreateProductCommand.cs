using Lunavex.Result;
using MediatR;

namespace ProductManager.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Yeni bir ürün oluþturmak için kullanýlan komut.
/// </summary>
/// <param name="Name">Ürünün adý.</param>
/// <param name="Price">Ürünün fiyatý.</param>
/// <param name="Description">Ürünün açýklamasý.</param>
public sealed record CreateProductCommand(string Name, decimal Price, string Description) : IRequest<Result<Guid>>;
