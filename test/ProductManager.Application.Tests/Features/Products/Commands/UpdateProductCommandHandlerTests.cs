using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Abstractions;
using ProductManager.Application.Features.Products.Commands.UpdateProduct;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Repositories;

// Test s�n�f�: UpdateProductCommandHandler i�in testler
public class UpdateProductCommandHandlerTests
{
    // Mock nesneler: Testlerde kullan�lan sahte (mock) nesneler.
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;
    private readonly UpdateProductCommandHandler _handler;

    // Constructor (yap�c� metod): Test s�n�f� ba�lat�l�rken mock nesneler olu�turuluyor
    // ve handler (i�lemci) �rne�i bu mock nesnelerle �al��acak �ekilde ayarlan�yor.
    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();

        // UpdateProductCommandHandler s�n�f�n�n bir �rne�i olu�turuluyor.
        _handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // Test: E�er �r�n bulunamazsa ne olur?
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange (Haz�rl�k a�amas�)
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");

        // Repository'deki GetByIdAsync metodu ayarlan�yor.
        // Bu metod �a�r�ld���nda null d�necek (yani �r�n bulunam�yor).
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product)null);  // �r�n bulunam�yor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve �r�n g�ncelleme i�lemi ba�lat�l�yor.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�s�z olmal�, ��nk� �r�n bulunamad�.
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("Product not found.");

        // Logger'�n "Product not found" mesaj�n� kaydedip kaydetmedi�ini kontrol ediyoruz.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {command.Id} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);  // Logger'�n bu uyar�y� sadece bir kere kaydetmesi bekleniyor.

        // �r�n bulunamad��� i�in UpdateAsync metodunun �a�r�lmad���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);

        // De�i�ikliklerin veritaban�na kaydedilmedi�ini do�ruluyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // Test: E�er �r�n ba�ar�yla g�ncellenirse ne olur?
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProductIsUpdatedSuccessfully()
    {
        // Arrange (Haz�rl�k a�amas�)
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");

        // Mevcut bir �r�n olu�turuluyor (eski bilgilerle).
        var product = new Product { Name = "OldProduct", Price = 10.99m, Description = "Old description" };

        // Repository'deki GetByIdAsync metodu ayarlan�yor. Bu metod �a�r�ld���nda �r�n bulunuyor.
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // �r�n bulundu

        // Mapper ayarlan�yor. Komuttaki yeni bilgiler ile mevcut �r�n g�ncelleniyor.
        _mapperMock
            .Setup(x => x.Map(command, product));  // Mapper kullan�larak �r�n g�ncelleniyor

        // Act (��lemi �al��t�r)
        // Handler'�n Handle metodu �a�r�l�yor ve �r�n g�ncelleme i�lemi ba�lat�l�yor.
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert (Sonucu kontrol et)
        // ��lem ba�ar�l� olmal�, ��nk� �r�n g�ncellendi.
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be($"Product with ID {product.Id} updated successfully.");

        // �r�n g�ncelleme i�leminin �a�r�ld���n� do�ruluyoruz.
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);

        // De�i�ikliklerin veritaban�na kaydedildi�ini do�ruluyoruz.
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSavingProductFails()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "UpdatedProduct", 19.99m, "Updated description");
        var product = new Product { Name = "OldProduct", Price = 10.99m, Description = "Old description" };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);  // �r�n bulundu

        _mapperMock
            .Setup(x => x.Map(command, product));  // �r�n mapper ile g�ncelleniyor

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));  // Veritaban� hatas�

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessages.Should().Contain("An error occurred while updating the product: Database error");

        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
