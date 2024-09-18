using Lunavex.Result;
using MediatR;
using ProductManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManager.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// <see cref="DeleteProductCommand"/> iþlemini yöneten sýnýf.
/// </summary>
public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    /// <summary>
    /// <see cref="DeleteProductCommandHandler"/> classýn yeni bir instancesi oluþturur
    /// </summary>
    /// <param name="productRepository">Ürün deposu.</param>
    /// <param name="unitOfWork">Ýþlemleri yönetmek için birim çalýþma.</param>
    /// <param name="logger">Bilgi kaydetmek için logger.</param>
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
    /// Mevcut bir ürünü silme iþlemini gerçekleþtirir.
    /// </summary>
    /// <param name="request">Silme komutu isteði.</param>
    /// <param name="cancellationToken">Ýptal belirteci.</param>
    /// <returns>Baþarýlý bir mesaj veya hata detaylarýný içeren sonuç.</returns>
    public async Task<Result<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Ürünün var olup olmadýðýný kontrol et
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            // Ürün bulunamadýysa uyarý kaydet
            _logger.LogWarning($"Product with ID {request.Id} not found.");
            return Result<string>.Failure(404, "Product not found.");
        }

        // Ürünü depodan sil
        await _productRepository.DeleteAsync(product.Id);

        // Deðiþiklikleri veritabanýna kaydet (iþlemi tamamla)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Baþarýlý sonucu döndür
        return Result<string>.Succeed($"Product with ID {product.Id} deleted successfully.");
    }
}