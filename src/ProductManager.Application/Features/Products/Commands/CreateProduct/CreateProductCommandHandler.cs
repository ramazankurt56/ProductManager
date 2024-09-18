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
    /// <see cref="CreateProductCommand"/> i�lemini y�neten s�n�f.
    /// </summary>
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        /// <summary>
        /// <see cref="CreateProductCommandHandler"/>  class�n�n yeni bir �rne�ini olu�turur.
        /// </summary>
        /// <param name="productRepository">�r�n deposu.</param>
        /// <param name="unitOfWork">��lemleri y�netmek i�in birim �al��ma.</param>
        /// <param name="mapper">Nesneler aras� d�n���m i�in mapper.</param>
        /// <param name="logger">Bilgi kaydetmek i�in logger.</param>
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
        /// Yeni bir �r�n olu�turma i�lemini ger�ekle�tirir.
        /// </summary>
        /// <param name="request">�r�n olu�turma komutu iste�i.</param>
        /// <param name="cancellationToken">�ptal belirteci.</param>
        /// <returns>Ba�ar�l� bir mesaj veya hata detaylar�n� i�eren sonu�.</returns>
        public async Task<Result<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Ayn� isimde bir �r�n olup olmad���n� kontrol et
            var existingProduct = await _productRepository.GetByNameAsync(request.Name);

            if (existingProduct != null)
            {
                // �r�n zaten varsa logla ve hata d�nd�r
                _logger.LogWarning($"A product with the name '{request.Name}' already exists.");
                return Result<string>.Failure(400, $"A product with the name '{request.Name}' already exists.");
            }

            // Komut iste�ini Product entity'sine d�n��t�r
            var product = _mapper.Map<Product>(request);

            // Yeni �r�n� depoya ekle
            await _productRepository.AddAsync(product);

            try
            {
                // De�i�iklikleri veritaban�na kaydet
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Ba�ar�l� sonucu d�nd�r
                return Result<string>.Succeed($"Product '{product.Name}' with ID {product.Id} created successfully.");
            }
            catch (Exception ex)
            {
                // Hata olu�tu�unda logla
                _logger.LogError(ex, $"An error occurred while saving the product {product.Name} to the database.");

                // Hata detaylar�yla birlikte ba�ar�s�z sonucu d�nd�r
                return Result<string>.Failure(500, $"An error occurred while creating the product: {ex.Message}");
            }
        }
    }
}
