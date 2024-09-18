using Lunavex.Result;
using MediatR;
using System;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Mevcut bir ürünü güncellemek için kullanýlan komut.
/// </summary>
/// <param name="Id">Ürünün benzersiz kimliði.</param>
/// <param name="Name">Ürünün güncellenmiþ adý.</param>
/// <param name="Price">Ürünün güncellenmiþ fiyatý.</param>
/// <param name="Description">Ürünün güncellenmiþ açýklamasý.</param>
public sealed record UpdateProductCommand(Guid Id, string Name, decimal Price, string Description) : IRequest<Result<string>>;
