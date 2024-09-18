using AutoMapper;
using Lunavex.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// <see cref="UpdateProductCommand"/> i�lemini y�neten class.
/// </summary>
public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    /// <summary>
    /// <see cref="UpdateProductCommandHandler"/> class�n yeni bir instancesi olu�turur
    /// </summary>
    /// <param name="productRepository">�r�n deposu.</param>
    /// <param name="unitOfWork">��lemleri y�netmek i�in birim �al��ma.</param>
    /// <param name="mapper">Nesneler aras� d�n���m i�in mapper.</param>
    /// <param name="logger">Bilgi kaydetmek i�in logger.</param>
    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Mevcut bir �r�n� g�ncelleme i�lemini ger�ekle�tirir.
    /// </summary>
    /// <param name="request">G�ncelleme komutu iste�i.</param>
    /// <param name="cancellationToken">�ptal belirteci.</param>
    /// <returns>Ba�ar�l� bir mesaj veya hata detaylar�n� i�eren sonu�.</returns>
    public async Task<Result<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // �r�n�n var olup olmad���n� kontrol et
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            // �r�n bulunamad�ysa uyar� kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<string>.Failure(404, "Product not found.");
        }

        // �r�n� yeni de�erlerle g�ncelle
        _mapper.Map(request, product);
        await _productRepository.UpdateAsync(product);
        try
        {
            // De�i�iklikleri veritaban�na kaydet
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<string>.Succeed($"Product with ID {product.Id} updated successfully.");
        }
        catch (Exception ex)
        {
            // Hata olu�ursa, logla ve hata mesaj� d�nd�r
            _logger.LogError(ex, $"An error occurred while updating the product: {ex.Message}");
            return Result<string>.Failure(500, $"An error occurred while updating the product: {ex.Message}");
        }
    }
}

