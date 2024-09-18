using Lunavex.Result;
using MediatR;
using ProductManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// <see cref="DeleteProductCommand"/> i�lemini y�neten s�n�f.
/// </summary>
public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    /// <summary>
    /// <see cref="DeleteProductCommandHandler"/> class�n yeni bir instancesi olu�turur
    /// </summary>
    /// <param name="productRepository">�r�n deposu.</param>
    /// <param name="unitOfWork">��lemleri y�netmek i�in birim �al��ma.</param>
    /// <param name="logger">Bilgi kaydetmek i�in logger.</param>
    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Mevcut bir �r�n� silme i�lemini ger�ekle�tirir.
    /// </summary>
    /// <param name="request">Silme komutu iste�i.</param>
    /// <param name="cancellationToken">�ptal belirteci.</param>
    /// <returns>Ba�ar�l� bir mesaj veya hata detaylar�n� i�eren sonu�.</returns>
    public async Task<Result<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // �r�n�n var olup olmad���n� kontrol et
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            // �r�n bulunamad�ysa uyar� kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<string>.Failure(404, "Product not found.");
        }

        // �r�n� depodan sil
        await _productRepository.DeleteAsync(product.Id);

        // De�i�iklikleri veritaban�na kaydet (i�lemi tamamla)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Ba�ar�l� sonucu d�nd�r
        return Result<string>.Succeed($"Product with ID {product.Id} deleted successfully.");
    }
}