using AutoMapper;
using Lunavex.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Models;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// <see cref="GetProductByIdQuery"/> i�lemini y�neten s�n�f.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    /// <summary>
    /// <see cref="GetProductByIdQueryHandler"/> class�n yeni bir instancesi olu�turur
    /// </summary>
    /// <param name="productRepository">�r�n deposu.</param>
    /// <param name="mapper">Nesneler aras� d�n���m i�in mapper.</param>
    /// <param name="logger">Bilgi kaydetmek i�in logger.</param>
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
    /// Bir �r�n� ID'sine g�re alma i�lemini ger�ekle�tirir.
    /// </summary>
    /// <param name="request">�r�n� ID'ye g�re alma sorgusu iste�i.</param>
    /// <param name="cancellationToken">�ptal belirteci.</param>
    /// <returns>�r�n DTO'su veya hata mesaj� d�nd�r�r.</returns>
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // �r�n� depodan ID'ye g�re al
        var product = await _productRepository.GetByIdAsync(request.Id);

        // �r�n bulunamad�ysa hata d�nd�r
        if (product == null)
        {
            // �r�n bulunamad���na dair uyar� kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<ProductDto>.Failure(404, "Product not found.");
        }

        // �r�n� ProductDto'ya d�n��t�r
        var productDto = _mapper.Map<ProductDto>(product);

        // �r�n DTO'sunu d�nd�r
        return Result<ProductDto>.Succeed(productDto);
    }
}
