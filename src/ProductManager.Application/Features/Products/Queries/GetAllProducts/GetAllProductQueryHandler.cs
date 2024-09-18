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
/// <see cref="GetAllProductQuery"/> i�lemini y�neten s�n�f.
/// </summary>
public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, Result<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllProductQueryHandler> _logger;

    /// <summary>
    /// <see cref="GetAllProductQueryHandler"/> class�n yeni bir instancesi olu�turur
    /// </summary>
    /// <param name="productRepository">�r�n deposu.</param>
    /// <param name="mapper">Nesneler aras� d�n���m i�in mapper.</param>
    /// <param name="logger">Bilgi kaydetmek i�in logger.</param>
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
    /// T�m �r�nleri alma i�lemini ger�ekle�tirir.
    /// </summary>
    /// <param name="request">T�m �r�nleri alma sorgusu iste�i.</param>
    /// <param name="cancellationToken">�ptal belirteci.</param>
    /// <returns>�r�n DTO'lar�n� i�eren bir liste veya bo� bir liste d�nd�r�r.</returns>
    public async Task<Result<List<ProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        // T�m �r�nleri veri kayna��ndan al
        var products = await _productRepository.GetAllAsync();

        // E�er �r�n yoksa, bo� bir liste d�nd�r
        if (products == null || products.Count == 0)
        {
            // �r�n bulunamad���na dair uyar� kaydet
            _logger.LogWarning("No products found.");
            return Result<List<ProductDto>>.Succeed(new List<ProductDto>());
        }

        // �r�nleri ProductDto'ya d�n��t�r
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        // �r�n DTO'lar�n� d�nd�r
        return Result<List<ProductDto>>.Succeed(productDtos);
    }
}

