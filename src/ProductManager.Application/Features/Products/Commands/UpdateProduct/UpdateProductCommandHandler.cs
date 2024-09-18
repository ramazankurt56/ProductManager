using AutoMapper;
using Lunavex.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Repositories;

namespace ProductManager.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// <see cref="UpdateProductCommand"/> iþlemini yöneten class.
/// </summary>
public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    /// <summary>
    /// <see cref="UpdateProductCommandHandler"/> classýn yeni bir instancesi oluþturur
    /// </summary>
    /// <param name="productRepository">Ürün deposu.</param>
    /// <param name="unitOfWork">Ýþlemleri yönetmek için birim çalýþma.</param>
    /// <param name="mapper">Nesneler arasý dönüþüm için mapper.</param>
    /// <param name="logger">Bilgi kaydetmek için logger.</param>
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
    /// Mevcut bir ürünü güncelleme iþlemini gerçekleþtirir.
    /// </summary>
    /// <param name="request">Güncelleme komutu isteði.</param>
    /// <param name="cancellationToken">Ýptal belirteci.</param>
    /// <returns>Baþarýlý bir mesaj veya hata detaylarýný içeren sonuç.</returns>
    public async Task<Result<string>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Ürünün var olup olmadýðýný kontrol et
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            // Ürün bulunamadýysa uyarý kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<string>.Failure(404, "Product not found.");
        }

        // Ürünü yeni deðerlerle güncelle
        _mapper.Map(request, product);
        await _productRepository.UpdateAsync(product);
        try
        {
            // Deðiþiklikleri veritabanýna kaydet
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<string>.Succeed($"Product with ID {product.Id} updated successfully.");
        }
        catch (Exception ex)
        {
            // Hata oluþursa, logla ve hata mesajý döndür
            _logger.LogError(ex, $"An error occurred while updating the product: {ex.Message}");
            return Result<string>.Failure(500, $"An error occurred while updating the product: {ex.Message}");
        }
    }
}

