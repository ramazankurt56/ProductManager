using AutoMapper;
using Lunavex.Result;
using MediatR;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ProductManager.Application.Features.Products.Commands.CreateProduct
{
    /// <summary>
    /// <see cref="CreateProductCommand"/> iþlemini yöneten sýnýf.
    /// </summary>
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        /// <summary>
        /// <see cref="CreateProductCommandHandler"/>  classýnýn yeni bir örneðini oluþturur.
        /// </summary>
        /// <param name="productRepository">Ürün deposu.</param>
        /// <param name="unitOfWork">Ýþlemleri yönetmek için birim çalýþma.</param>
        /// <param name="mapper">Nesneler arasý dönüþüm için mapper.</param>
        /// <param name="logger">Bilgi kaydetmek için logger.</param>
        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Yeni bir ürün oluþturma iþlemini gerçekleþtirir.
        /// </summary>
        /// <param name="request">Ürün oluþturma komutu isteði.</param>
        /// <param name="cancellationToken">Ýptal belirteci.</param>
        /// <returns>Baþarýlý bir mesaj veya hata detaylarýný içeren sonuç.</returns>
        public async Task<Result<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Ayný isimde bir ürün olup olmadýðýný kontrol et
            var existingProduct = await _productRepository.GetByNameAsync(request.Name);

            if (existingProduct != null)
            {
                // Ürün zaten varsa logla ve hata döndür
                _logger.LogWarning($"A product with the name '{request.Name}' already exists.");
                return Result<string>.Failure(400, $"A product with the name '{request.Name}' already exists.");
            }

            // Komut isteðini Product entity'sine dönüþtür
            var product = _mapper.Map<Product>(request);

            // Yeni ürünü depoya ekle
            await _productRepository.AddAsync(product);

            try
            {
                // Deðiþiklikleri veritabanýna kaydet
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Baþarýlý sonucu döndür
                return Result<string>.Succeed($"Product '{product.Name}' with ID {product.Id} created successfully.");
            }
            catch (Exception ex)
            {
                // Hata oluþtuðunda logla
                _logger.LogError(ex, $"An error occurred while saving the product {product.Name} to the database.");

                // Hata detaylarýyla birlikte baþarýsýz sonucu döndür
                return Result<string>.Failure(500, $"An error occurred while creating the product: {ex.Message}");
            }
        }
    }
}
