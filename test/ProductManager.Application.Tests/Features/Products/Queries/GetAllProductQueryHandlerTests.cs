using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Features.Products.Queries.GetAllProducts;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

// Test sýnýfý: GetAllProductQueryHandler için testler
public class GetAllProductQueryHandlerTests
{
    // Mock nesneler: Testlerde kullanýlan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAllProductQueryHandler>> _loggerMock;
    private readonly GetAllProductQueryHandler _handler;

    // Constructor (yapýcý metod): Test sýnýfý baþlatýlýrken mock nesneler oluþturuluyor
    // ve handler (iþlemci) örneði bu mock nesnelerle çalýþacak þekilde ayarlanýyor.
    public GetAllProductQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAllProductQueryHandler>>();

        // GetAllProductQueryHandler sýnýfýnýn bir örneði oluþturuluyor.
        _handler = new GetAllProductQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: Eðer ürün bulunmazsa ne olur?
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange (Hazýrlýk aþamasý)
        // Repository'deki GetAllAsync metodu ayarlanýyor.
        // Bu metod çaðrýldýðýnda boþ bir ürün listesi döndürülecek.
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>());  // Boþ ürün listesi döndürülüyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve sorgu iþleniyor.
        var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýlý olmalý, ama dönen ürün listesi boþ olmalý.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().BeEmpty();

        // Logger'ýn "No products found" mesajýný kaydedip kaydetmediðini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No products found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'ýn bu uyarýyý sadece bir kere kaydetmesi bekleniyor.

        // Repository'nin GetAllAsync metodunun bir kez çaðrýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    // Test: Ürünler bulunduðunda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnProductDtos_WhenProductsFound()
    {
        // Arrange (Hazýrlýk aþamasý)
        // Ürünlerin olduðu bir liste oluþturuluyor.
        var products = new List<Product>
        {
            new Product { Name = "Product 1" },
            new Product { Name = "Product 2" }
        };

        // Ürün DTO'larýnýn olduðu bir liste oluþturuluyor.
        var productDtos = new List<ProductDto>
        {
            new ProductDto { Name = "Product 1" },
            new ProductDto { Name = "Product 2" }
        };

        // Repository'deki GetAllAsync metodu ayarlanýyor.
        // Bu metod çaðrýldýðýnda, ürün listesi döndürülecek.
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);  // Ürün listesi döndürülüyor

        // Mapper ayarlanýyor. Ürünler DTO'ya dönüþtürülüyor.
        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(products))
            .Returns(productDtos);  // Ürünler DTO'ya dönüþtürülüyor

        // Act (Ýþlemi çalýþtýr)
        // Handler'ýn Handle metodu çaðrýlýyor ve sorgu iþleniyor.
        var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // Ýþlem baþarýlý olmalý ve ürün DTO'larýnýn listesi döndürülmeli.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().HaveCount(2);  // Ýki ürün bulunmalý
        result.Data.Should().BeEquivalentTo(productDtos);  // Dönen veriler DTO'larla eþleþmeli.

        // Repository'nin GetAllAsync metodunun bir kez çaðrýldýðýný doðruluyoruz.
        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);

        // Mapper'ýn, ürünleri DTO'ya dönüþtürmek için bir kez çaðrýldýðýný doðruluyoruz.
        _mapperMock.Verify(x => x.Map<List<ProductDto>>(products), Times.Once);
    }
}
