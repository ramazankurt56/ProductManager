using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Features.Products.Queries.GetProductById;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
// Test sýnýfý: GetProductByIdQueryHandler için testler
public class GetProductByIdQueryHandlerTests
{
    // Mock nesneler: Testlerde gerçek nesneler yerine kullanýlan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetProductByIdQueryHandler>> _loggerMock;
    private readonly GetProductByIdQueryHandler _handler;

    // Constructor (yapýcý metod): Test sýnýfý baþlatýlýrken mock nesneler oluþturuluyor
    // ve handler (iþlemci) örneði bu mock nesnelerle çalýþacak þekilde ayarlanýyor.
    public GetProductByIdQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetProductByIdQueryHandler>>();

        // GetProductByIdQueryHandler sýnýfýnýn bir örneði oluþturuluyor.
        _handler = new GetProductByIdQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: Ürün bulunamadýðýnda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange (Hazýrlýk aþamasý)
        // Bir sorgu oluþturuluyor (ürünün ID'si ile).
        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Repository'deki GetByIdAsync metodu ayarlanýyor.
        // Bu metod çaðrýldýðýnda null dönecek þekilde ayarlanýyor (yani ürün bulunamýyor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // Ürün bulunamýyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve sorgu iþleniyor.
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýsýz olmalý, çünkü ürün bulunamadý.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'ýn "Product not found" mesajýný kaydedip kaydetmediðini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {query.Id} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'ýn bu uyarýyý sadece bir kere kaydetmesi bekleniyor.

        // Repository'nin GetByIdAsync metodunun bir kez çaðrýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    // Test: Ürün bulunduðunda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnProductDto_WhenProductIsFound()
    {
        // Arrange (Hazýrlýk aþamasý)
        // Sorguyu hazýrlýyoruz (ürünün ID'si ile).
        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Ürünün manuel olarak bir örneði oluþturuluyor.
        var product = new Product { Name = "TestProduct" };

        // Ürünün DTO'su (veri transfer nesnesi) oluþturuluyor.
        var productDto = new ProductDto { Id = query.Id, Name = "TestProduct" };

        // Repository'deki GetByIdAsync metodu ayarlanýyor.
        // Bu metod çaðrýldýðýnda product dönecek (ürün bulunuyor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // Ürün bulundu

        // Mapper ayarlanýyor. Product nesnesi ProductDto'ya dönüþtürülüyor.
        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(productDto);  // Ürün DTO'ya dönüþtürülüyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve sorgu iþleniyor.
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýlý olmalý, çünkü ürün bulundu.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(productDto);  // Dönen veriler, DTO'ya eþit olmalý.

        // Repository'nin GetByIdAsync metodunun bir kez çaðrýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

        // Mapper'ýn, Product nesnesini ProductDto'ya dönüþtürmek için bir kez çaðrýldýðýný doðruluyoruz.
        _mapperMock.Verify(x => x.Map<ProductDto>(product), Times.Once);
    }
}
