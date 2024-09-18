using AutoMapper;
using Lunavex.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Models;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// <see cref="GetProductByIdQuery"/> iþlemini yöneten sýnýf.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    /// <summary>
    /// <see cref="GetProductByIdQueryHandler"/> classýn yeni bir instancesi oluþturur
    /// </summary>
    /// <param name="productRepository">Ürün deposu.</param>
    /// <param name="mapper">Nesneler arasý dönüþüm için mapper.</param>
    /// <param name="logger">Bilgi kaydetmek için logger.</param>
    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Bir ürünü ID'sine göre alma iþlemini gerçekleþtirir.
    /// </summary>
    /// <param name="request">Ürünü ID'ye göre alma sorgusu isteði.</param>
    /// <param name="cancellationToken">Ýptal belirteci.</param>
    /// <returns>Ürün DTO'su veya hata mesajý döndürür.</returns>
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // Ürünü depodan ID'ye göre al
        var product = await _productRepository.GetByIdAsync(request.Id);

        // Ürün bulunamadýysa hata döndür
        if (product == null)
        {
            // Ürün bulunamadýðýna dair uyarý kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<ProductDto>.Failure(404, "Product not found.");
        }

        // Ürünü ProductDto'ya dönüþtür
        var productDto = _mapper.Map<ProductDto>(product);

        // Ürün DTO'sunu döndür
        return Result<ProductDto>.Succeed(productDto);
    }
}
