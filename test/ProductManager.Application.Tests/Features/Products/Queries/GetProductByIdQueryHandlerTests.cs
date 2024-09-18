using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Features.Products.Queries.GetProductById;
using ProductManager.Application.Models;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;
// Test s�n�f�: GetProductByIdQueryHandler i�in testler
public class GetProductByIdQueryHandlerTests
{
    // Mock nesneler: Testlerde ger�ek nesneler yerine kullan�lan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GetProductByIdQueryHandler>> _loggerMock;
    private readonly GetProductByIdQueryHandler _handler;

    // Constructor (yap�c� metod): Test s�n�f� ba�lat�l�rken mock nesneler olu�turuluyor
    // ve handler (i�lemci) �rne�i bu mock nesnelerle �al��acak �ekilde ayarlan�yor.
    public GetProductByIdQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GetProductByIdQueryHandler>>();

        // GetProductByIdQueryHandler s�n�f�n�n bir �rne�i olu�turuluyor.
        _handler = new GetProductByIdQueryHandler(
            _productRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: �r�n bulunamad���nda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange (Haz�rl�k a�amas�)
        // Bir sorgu olu�turuluyor (�r�n�n ID'si ile).
        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Repository'deki GetByIdAsync metodu ayarlan�yor.
        // Bu metod �a�r�ld���nda null d�necek �ekilde ayarlan�yor (yani �r�n bulunam�yor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // �r�n bulunam�yor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve sorgu i�leniyor.
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�s�z olmal�, ��nk� �r�n bulunamad�.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'�n "Product not found" mesaj�n� kaydedip kaydetmedi�ini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {query.Id} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'�n bu uyar�y� sadece bir kere kaydetmesi bekleniyor.

        // Repository'nin GetByIdAsync metodunun bir kez �a�r�ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    // Test: �r�n bulundu�unda ne olur?
    [Fact]
    public async Task Handle_ShouldReturnProductDto_WhenProductIsFound()
    {
        // Arrange (Haz�rl�k a�amas�)
        // Sorguyu haz�rl�yoruz (�r�n�n ID'si ile).
        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // �r�n�n manuel olarak bir �rne�i olu�turuluyor.
        var product = new Product { Name = "TestProduct" };

        // �r�n�n DTO'su (veri transfer nesnesi) olu�turuluyor.
        var productDto = new ProductDto { Id = query.Id, Name = "TestProduct" };

        // Repository'deki GetByIdAsync metodu ayarlan�yor.
        // Bu metod �a�r�ld���nda product d�necek (�r�n bulunuyor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // �r�n bulundu

        // Mapper ayarlan�yor. Product nesnesi ProductDto'ya d�n��t�r�l�yor.
        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(productDto);  // �r�n DTO'ya d�n��t�r�l�yor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve sorgu i�leniyor.
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�l� olmal�, ��nk� �r�n bulundu.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(productDto);  // D�nen veriler, DTO'ya e�it olmal�.

        // Repository'nin GetByIdAsync metodunun bir kez �a�r�ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

        // Mapper'�n, Product nesnesini ProductDto'ya d�n��t�rmek i�in bir kez �a�r�ld���n� do�ruluyoruz.
        _mapperMock.Verify(x => x.Map<ProductDto>(product), Times.Once);
    }
}
