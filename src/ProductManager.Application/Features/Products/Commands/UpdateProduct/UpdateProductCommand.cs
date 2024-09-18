using Lunavex.Result;
using MediatR;
using System;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Mevcut bir �r�n� g�ncellemek i�in kullan�lan komut.
/// </summary>
/// <param name="Id">�r�n�n benzersiz kimli�i.</param>
/// <param name="Name">�r�n�n g�ncellenmi� ad�.</param>
/// <param name="Price">�r�n�n g�ncellenmi� fiyat�.</param>
/// <param name="Description">�r�n�n g�ncellenmi� a��klamas�.</param>
public sealed record UpdateProductCommand(Guid Id, string Name, decimal Price, string Description) : IRequest<Result<string>>;
