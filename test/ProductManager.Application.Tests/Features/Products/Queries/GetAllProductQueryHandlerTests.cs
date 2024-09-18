using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Features.Products.Queries.GetAllProducts;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

// Test s�n�f�: GetAllProductQueryHandler i�in testler
public class GetAllProductQueryHandlerTests
{
    // Mock nesneler: Testlerde kullan�lan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetAllProductQueryHandler>> _loggerMock;
    private readonly GetAllProductQueryHandler _handler;

    // Constructor (yap�c� metod): Test s�n�f� ba�lat�l�rken mock nesneler olu�turuluyor
    // ve handler (i�lemci) �rne�i bu mock nesnelerle �al��acak �ekilde ayarlan�yor.
    public GetAllProductQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetAllProductQueryHandler>>();

        // GetAllProductQueryHandler s�n�f�n�n bir �rne�i olu�turuluyor.
        _handler = new GetAllProductQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: E�er �r�n bulunmazsa ne olur?
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        // Arrange (Haz�rl�k a�amas�)
        // Repository'deki GetAllAsync metodu ayarlan�yor.
        // Bu metod �a�r�ld���nda bo� bir �r�n listesi d�nd�r�lecek.
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>());  // Bo� �r�n listesi d�nd�r�l�yor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve sorgu i�leniyor.
        var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�l� olmal�, ama d�nen �r�n listesi bo� olmal�.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().BeEmpty();

        // Logger'�n "No products found" mesaj�n� kaydedip kaydetmedi�ini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No products found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'�n bu uyar�y� sadece bir kere kaydetmesi bekleniyor.

        // Repository'nin GetAllAsync metodunun bir kez �a�r�ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    // Test: �r�nler bulundu�unda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnProductDtos_WhenProductsFound()
    {
        // Arrange (Haz�rl�k a�amas�)
        // �r�nlerin oldu�u bir liste olu�turuluyor.
        var products = new List<Product>
        {
            new Product { Name = "Product 1" },
            new Product { Name = "Product 2" }
        };

        // �r�n DTO'lar�n�n oldu�u bir liste olu�turuluyor.
        var productDtos = new List<ProductDto>
        {
            new ProductDto { Name = "Product 1" },
            new ProductDto { Name = "Product 2" }
        };

        // Repository'deki GetAllAsync metodu ayarlan�yor.
        // Bu metod �a�r�ld���nda, �r�n listesi d�nd�r�lecek.
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);  // �r�n listesi d�nd�r�l�yor

        // Mapper ayarlan�yor. �r�nler DTO'ya d�n��t�r�l�yor.
        _mapperMock
            .Setup(x => x.Map<List<ProductDto>>(products))
            .Returns(productDtos);  // �r�nler DTO'ya d�n��t�r�l�yor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve sorgu i�leniyor.
        var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�l� olmal� ve �r�n DTO'lar�n�n listesi d�nd�r�lmeli.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().HaveCount(2);  // �ki �r�n bulunmal�
        result.Data.Should().BeEquivalentTo(productDtos);  // D�nen veriler DTO'larla e�le�meli.

        // Repository'nin GetAllAsync metodunun bir kez �a�r�ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);

        // Mapper'�n, �r�nleri DTO'ya d�n��t�rmek i�in bir kez �a�r�ld���n� do�ruluyoruz.
        _mapperMock.Verify(x => x.Map<List<ProductDto>>(products), Times.Once);
    }
}
