// Bu dosya, ürünlerle ilgili API işlemlerini yöneten ProductsController sınıfını tanımlar.

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Features.Products.Commands.CreateProduct;
using ProductManager.Application.Features.Products.Commands.DeleteProduct;
using ProductManager.Application.Features.Products.Commands.UpdateProduct;
using ProductManager.Application.Features.Products.Queries.GetAllProducts;
using ProductManager.Application.Features.Products.Queries.GetProductById;
using ProductManager.Application.Models;
using ProductManager.WebApi.Abstractions;

namespace ProductManager.WebApi.Controllers;

/// <summary>
/// Ürünlerle ilgili API işlemlerini yöneten controller.
/// </summary>
public class ProductsController : ApiController
{
    /// <summary>
    /// ProductsController sınıfının yapıcısı.
    /// </summary>
    /// <param name="mediator">MediatR arayüzü.</param>
    public ProductsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Yeni bir ürün oluşturur.
    /// </summary>
    /// <param name="command">Ürün oluşturma komutu.</param>
    /// <returns>Oluşturulan ürünün idsi </returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        // Komutu gönderir ve sonucu alır.
        var result = await _mediator.Send(command);

        // İşlem başarısız ise hata mesajlarını döndürür.
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result.ErrorMessages);
        }

        // Başarılı ise oluşturulan ürünün idsini döndürür.
        return Created("", result.Data);
    }

    /// <summary>
    /// Tüm ürünleri getirir.
    /// </summary>
    /// <returns>Ürünlerin listesi.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
        // Tüm ürünleri almak için sorguyu gönderir.
        var result = await _mediator.Send(new GetAllProductQuery());

        // İşlem başarısız ise hata mesajlarını döndürür.
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result.ErrorMessages);
        }

        // Başarılı ise ürün listesini döndürür.
        return Ok(result.Data);
    }

    /// <summary>
    /// Belirli bir ürünü ID'sine göre getirir.
    /// </summary>
    /// <param name="id">Ürünün benzersiz ID'si.</param>
    /// <returns>Ürünün detayları.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        // Ürünü ID'sine göre almak için sorguyu gönderir.
        var result = await _mediator.Send(new GetProductByIdQuery { Id = id });

        // İşlem başarısız ise hata mesajlarını döndürür.
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result.ErrorMessages);
        }

        // Başarılı ise ürünü döndürür.
        return Ok(result.Data);
    }

    /// <summary>
    /// Mevcut bir ürünü günceller.
    /// </summary>
    /// <param name="id">Güncellenecek ürünün ID'si.</param>
    /// <param name="request">Ürün güncelleme isteği.</param>
    /// <returns>İşlemin sonucu.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductDto request)
    {
        // Güncelleme komutunu oluşturur.
        var command = new UpdateProductCommand(id, request.Name, request.Price, request.Description);

        // Komutu gönderir ve sonucu alır.
        var result = await _mediator.Send(command);

        // İşlem başarısız ise hata mesajlarını döndürür.
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result.ErrorMessages);
        }

        // Başarılı ise 200 OK döndürür.
        return Ok();
    }

    /// <summary>
    /// Belirli bir ürünü siler.
    /// </summary>
    /// <param name="id">Silinecek ürünün ID'si.</param>
    /// <returns>İşlemin sonucu.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        // Silme komutunu gönderir ve sonucu alır.
        var result = await _mediator.Send(new DeleteProductCommand { Id = id });

        // İşlem başarısız ise hata mesajlarını döndürür.
        if (!result.IsSuccessful)
        {
            return StatusCode(result.StatusCode, result.ErrorMessages);
        }

        // Başarılı ise 200 OK döndürür.
        return Ok();
    }
}
