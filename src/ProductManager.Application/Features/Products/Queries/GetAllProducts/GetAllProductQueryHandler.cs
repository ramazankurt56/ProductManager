using AutoMapper;
using Lunavex.Result;
using MediatR;
using ProductManager.Application.Models;
using ProductManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManager.Application.Features.Products.Queries.GetAllProducts;

/// <summary>
/// <see cref="GetAllProductQuery"/> iþlemini yöneten sýnýf.
/// </summary>
public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, Result<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllProductQueryHandler> _logger;

    /// <summary>
    /// <see cref="GetAllProductQueryHandler"/> classýn yeni bir instancesi oluþturur
    /// </summary>
    /// <param name="productRepository">Ürün deposu.</param>
    /// <param name="mapper">Nesneler arasý dönüþüm için mapper.</param>
    /// <param name="logger">Bilgi kaydetmek için logger.</param>
    public GetAllProductQueryHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetAllProductQueryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Tüm ürünleri alma iþlemini gerçekleþtirir.
    /// </summary>
    /// <param name="request">Tüm ürünleri alma sorgusu isteði.</param>
    /// <param name="cancellationToken">Ýptal belirteci.</param>
    /// <returns>Ürün DTO'larýný içeren bir liste veya boþ bir liste döndürür.</returns>
    public async Task<Result<List<ProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        // Tüm ürünleri veri kaynaðýndan al
        var products = await _productRepository.GetAllAsync();

        // Eðer ürün yoksa, boþ bir liste döndür
        if (products == null || products.Count == 0)
        {
            // Ürün bulunamadýðýna dair uyarý kaydet
            _logger.LogWarning("No products found.");
            return Result<List<ProductDto>>.Succeed(new List<ProductDto>());
        }

        // Ürünleri ProductDto'ya dönüþtür
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        // Ürün DTO'larýný döndür
        return Result<List<ProductDto>>.Succeed(productDtos);
    }
}

